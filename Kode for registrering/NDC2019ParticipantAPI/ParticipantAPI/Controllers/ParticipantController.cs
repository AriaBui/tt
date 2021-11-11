using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ParticipantAPI.Configuration;
using ParticipantAPI.Models;
using ParticipantAPI.Services;
using System.Linq;

namespace ParticipantAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParticipantController : ControllerBase
    {
        private readonly IParticipantStorageService _storageService;

        public ParticipantController(IParticipantStorageService storageService)
        {
            _storageService = storageService;
        }

        // GET api/participant/ping
        [HttpGet("Ping")]
        public async Task<IActionResult> Ping()
        {
            return Ok();
        }

        [Route("bedex")]
        [HttpPost]
        public async Task<IActionResult> Bedex([FromBody]BedexScore bedexScore)
        {
            await _storageService.Add(new Participant(Guid.NewGuid().ToString(), true)
            {
                FirstName = bedexScore.Name,
                Score = bedexScore.Score
            });
            return Ok();
        }

        [Route("javazone")]
        [HttpPost]
        public async Task<IActionResult> Javazone([FromBody]JavaZoneQrCodeData jzone)
        {
            await _storageService.Add(new Participant(jzone.Mail, true)
            {
                FirstName = jzone.Name,
                Company = jzone.Company,
                Email = jzone.Mail,
                PhoneNumber = jzone.Phone,
                Score = jzone.Score,
                WorkAddress1 = jzone.Id
            });
            return Ok();
        }

        [Route("fulldump")]
        [HttpPost]
        public async Task<IActionResult> Javazone([FromBody] TextBackup text)
        {
            await _storageService.Add(new Participant(Guid.NewGuid().ToString(), true)
            {
                Score = 0,
                WorkAddress2 = text.Data
            });
            return Ok();
        }


        // GET api/participant/updateParticipantData
        [HttpGet("UpdateParticipantData")]
        public async Task<IActionResult> UpdateParticipantData()
        {
            var participants = (List<Participant>)await _storageService.GetAll();
            var participantsToUpdate = participants.Where(x => x.FirstName == null && x.HasTriedUpdate == false).ToList();

            var client = new HttpClient();

            foreach (var part in participantsToUpdate)
            {
                part.HasTriedUpdate = true;
                await _storageService.Update(part);

                var response = await client.GetAsync(string.Concat("https://ndc-conference-api.herokuapp.com/", part.AttendeeId));
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var participant = JsonConvert.DeserializeObject<Participant>(content);
                    participant.HasTriedUpdate = true;
                    var result = await _storageService.Update(participant);

                    if (result == false)
                    {
                        // do something?
                    }
                }
            }

            return Ok();
        }

        // GET api/participant/5    9015115991124874811001
        [HttpPost("{id}/{assentGDPR}/enqueue")]
        public async Task<ActionResult> Enqueue(string id, bool? assentGDPR)
        {
            var participant = new Participant(id);

            if (assentGDPR.HasValue)
            {
                participant.AssentGDPR = assentGDPR.Value;
            }
            var result = await _storageService.Add(participant);

            if (result == false)
            {
                return Conflict();
            }
            return NoContent();
        }

        // GET api/participant/5    9015115991124874811001
        [HttpPost("{id}/{assentGDPR}")]
        public async Task<ActionResult> Create(string id, bool? assentGDPR)
        {
            if (!assentGDPR.HasValue)
            {
                assentGDPR = false;
            }
            var addParticipant = await _storageService.Add(new Participant(id, assentGDPR.Value));
            
            var client = new HttpClient();
            var response = await client.GetAsync(string.Concat("https://ndc-conference-api.herokuapp.com/", id));
            var content = await response.Content.ReadAsStringAsync();

            var participant = JsonConvert.DeserializeObject<Participant>(content);
            participant.AssentGDPR = assentGDPR.Value;

            var result = await _storageService.Update(participant);

            if (result == false)
            {
                return Conflict();
            }
            return Ok(participant);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var participants = await _storageService.GetAll();
            if(participants == null)
            {
                return NotFound();
            }
            return Ok(participants);
        }

        [HttpGet("{id}", Name = "GetParticipant")]
        public async Task<IActionResult> Get(string id)
        {
            var participant = await _storageService.Get(AzureTableSettings.PARTITION_KEY, id);
            if (participant == null)
            {
                return NotFound();
            }
            return Ok(participant);
        }

        [HttpPut("{id}/{score}", Name = "Update")]
        public async Task<IActionResult> Update(string id, long score)
        {
            var particiant = await _storageService.Get(AzureTableSettings.PARTITION_KEY, id);
            if(particiant == null)
            {
                return NotFound();
            }

            if(score <= particiant.Score)
            {
                return NoContent();
            }

            particiant.Score = score;

            var result = await _storageService.Update(particiant);
            if (result == false)
            {
                return UnprocessableEntity();
            }
            return NoContent();
        }
    }
}
