using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;

namespace oostfraeiskorg.ViewModels;

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

    private readonly string BearerToken;
    private readonly string SmtpCredentialName;
    private readonly string SmtpCredentialPassword;

    public DonateawordViewModel(IConfiguration configuration)
    {
        // Load settings
        SmtpCredentialName = configuration["TranslatorConfig:SmtpCredentialName"];
        if (string.IsNullOrEmpty(SmtpCredentialName))
        {
            throw new Exception("SmtpCredentialName is missing in the configuration.");
        }
        SmtpCredentialPassword = configuration["TranslatorConfig:SmtpCredentialPassword"];
        if (string.IsNullOrEmpty(SmtpCredentialPassword))
        {
            throw new Exception("SmtpCredentialPassword is missing in the configuration.");
        }
    }

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
            Credentials = new NetworkCredential(SmtpCredentialName, SmtpCredentialPassword)
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

