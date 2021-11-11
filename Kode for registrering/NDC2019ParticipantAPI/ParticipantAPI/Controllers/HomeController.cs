using Microsoft.AspNetCore.Mvc;
using ParticipantAPI.Models;
using ParticipantAPI.Services;
using ParticipantAPI.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParticipantAPI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IParticipantStorageService _storageService;

        public HomeController(IParticipantStorageService storageService)
        {
            _storageService = storageService;
        }
        public async Task<IActionResult> Index()
        {
            var participants = await _storageService.GetAll();
            if (participants == null)
            {
                return NotFound();
            }
            var scores = Map(participants).OrderByDescending(x => x.Score).ToList();
            return View(scores);
        }

        private IList<HighScore> Map(IEnumerable<Participant> participants)
        {
            var scores = new List<HighScore>();
            foreach (var participant in participants)
            {
                scores.Add(new HighScore
                {
                    Name = string.Concat(participant.FirstName, " ", participant.Surname),
                    Score = participant.Score
                });
            }

            return scores;
        }
    }
}