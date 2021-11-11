using System;
using VismaKart.Models;

namespace VismaKart.UtilsExtra
{
    public static class JavaZoneQrParser
    {
        public static JavaZoneQrCodeData ParseQrCode(string qrcode)
        {
            // MECARD:N:Vebjørn R Olsen;EMAIL:vro@macsimum.no;ORG:Macsimum;UID:UK3xJswpF+oUNj+d9HE10w==;;
            // BEGIN:VCARD VERSION:2.1 N:KvÃ¦rna;Halvard FN:Halvard KvÃ¦rna TEL;CELL:+4741437947 EMAIL:halvard.hella.kvaerna@visma.com BMSTID:57334741 END:VCARD

            try
            {
                if (qrcode.Contains("MECARD:"))
                {
                    return ParseMeCard(qrcode);
                }

                if (qrcode.Contains("BEGIN:VCARD"))
                {
                    return ParseVCard(qrcode);
                }
            }
            catch
            {
                return new JavaZoneQrCodeData
                {
                    Id = qrcode
                };
            }

            return null;
        }

        private static JavaZoneQrCodeData ParseVCard(string qrcode)
        {
            qrcode = qrcode.Replace("BEGIN:VCARD", string.Empty);
            qrcode = qrcode.Replace("END:VCARD", string.Empty);
            qrcode = qrcode.Replace("VERSION:2.1", string.Empty);
            qrcode = qrcode.Trim();

            var lines = qrcode.Split(
                new[] {"N:", "FN:", "TEL;CELL:", "EMAIL:", "BMSTID:"},
                StringSplitOptions.RemoveEmptyEntries);

            var n = lines[0]?.Trim();
            var fn = lines[1]?.Trim();
            var cell = lines[2]?.Trim();
            var mail = lines[3]?.Trim();

            var data = new JavaZoneQrCodeData();

            data.Id = qrcode;
            data.Name = fn;
            data.Phone = cell;
            data.Mail = mail;

            return data;
        }

        private static JavaZoneQrCodeData ParseMeCard(string qrcode)
        {
            qrcode = qrcode.Replace("MECARD:", string.Empty);

            var data = new JavaZoneQrCodeData();
            data.Id = qrcode;

            var arr = qrcode.Split(';');
            foreach (var line in arr)
            {
                var trimmedLine = line.Trim();
                if (TryParseCode(trimmedLine, "N", out var name))
                {
                    data.Name = name;
                }

                // Tror ikke QR-koden inneholder TEL, men i følge standarden kan det være der
                else if (TryParseCode(trimmedLine, "TEL", out var tel))
                {
                    data.Phone = tel;
                }
                else if (TryParseCode(trimmedLine, "EMAIL", out var mail))
                {
                    data.Mail = mail;
                }
                else if (TryParseCode(trimmedLine, "ORG", out var company))
                {
                    data.Company = company;
                }
            }
            return data;
        }

        private static bool TryParseCode(string line, string type, out string value)
        {
            value = string.Empty;
            if (!line.StartsWith(type)) return false;

            value = line.Replace($"{type}:", string.Empty);

            return true;
        }
    }
}
