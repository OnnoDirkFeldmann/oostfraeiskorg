using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotVVM.Framework.ViewModel;
using DotVVM.Framework.Hosting;
using System.Diagnostics.Metrics;
using System.Net.Mail;
using System.Net;

namespace oostfraeiskorg.ViewModels
{
    public class DonateawordViewModel : MasterPageViewModel
    {
        public string DonatedWordWord { get; set; }

        public string DonatedWordGermanTranslation { get; set; }

        public string DonatedWordPlace { get; set; }

        public string DonatedWordName { get; set; }

        public string DonatedWordBirth { get; set; }

        public string DonatedWordHome { get; set; }

        public string DonatedWordEmail { get; set; }

        public string DonatedWordOther { get; set; }

        public void DonateWord()
        {

            if (string.IsNullOrWhiteSpace(DonatedWordWord))
            {
                AlertText = "Wort ist leer";
                AlertVisible = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(DonatedWordGermanTranslation))
            {
                AlertText = "Übersetzung ist leer";
                AlertVisible = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(DonatedWordPlace))
            {
                AlertText = "Gegend ist leer";
                AlertVisible = true;
                return;
            }

            var smtpClient = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("edufraeisk@gmail.com", "aqaxxjjwedmtvuic")
            };
            var mailMessage = new MailMessage
            {
                From = new MailAddress("edufraeisk@gmail.com"),
                Subject = $"Wort '{DonatedWordWord}' gespendet",
                Body = $"Wort:{DonatedWordWord}<br/>" +
                $"Deutsche Übersetzung:{DonatedWordGermanTranslation}<br/>" +
                $"Gegend:{DonatedWordPlace}<br/>" +
                $"Name:{DonatedWordName}<br/>" +
                $"Geburtsjahr:{DonatedWordBirth}<br/>" +
                $"Wohnort:{DonatedWordHome}<br/>" +
                $"Email:{DonatedWordEmail}<br/>" +
                $"Sonstiges:{DonatedWordOther}<br/>",
                IsBodyHtml = true,
            };
            mailMessage.To.Add("edufraeisk@gmail.com");

            smtpClient.Send(mailMessage);

            AlertText = "Wortspende erfolgreich versendet. Vielen Dank!";
            AlertVisible = true;
        }
    }
}

