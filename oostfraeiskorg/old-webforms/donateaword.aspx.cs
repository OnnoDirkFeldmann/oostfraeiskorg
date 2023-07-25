using System;
using System.Net;
using System.Net.Mail;
using System.Web.UI.WebControls;

namespace WFDOT
{
    public partial class donateaword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.Page.Title = "Ein Wort spenden - Ōstfräisk Wōrdenbauk - Ostfriesisches Wörterbuch";
            Master.Page.MetaKeywords += "spende ein Wort, donate a word, jungfräiske mäinskup, jfm, ostfriesische Sprache, ostfriesisch, ōstfräisk, links";
            Master.Page.MetaDescription = "Ein Wort spenden - Wörterbuch der ostfriesischen Sprache - Wörter aus dem Ostfriesischen oder ins Ostfriesische übersetzen. Die Sprache der Ostfriesen mit dem Wörterbuch für das ostfriesische Platt als Standardostfriesisch lernen.";
        }

        protected void btn_submit_Click(object sender, EventArgs e)
        {
            var alert = (Label)Master.FindControl("lblAlert");

            if (string.IsNullOrWhiteSpace(txt_word.Text))
            {
                alert.Text = "Wort ist leer";
                alert.Visible = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(txt_german.Text))
            {
                alert.Text = "Übersetzung ist leer";
                alert.Visible = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(txt_place.Text))
            {
                alert.Text = "Gegend ist leer";
                alert.Visible = true;
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
                Subject = $"Wort '{txt_word.Text}' gespendet",
                Body = $"Wort:{txt_word.Text}<br/>" +
                $"Deutsche Übersetzung:{txt_german.Text}<br/>" +
                $"Gegend:{txt_place.Text}<br/>" +
                $"Name:{txt_birth.Text}<br/>" +
                $"Geburtsjahr:{txt_birth.Text}<br/>" +
                $"Wohnort:{txt_home.Text}<br/>" +
                $"Email:{txt_email.Text}<br/>" +
                $"Sonstiges:{txt_other.Text}<br/>",
                IsBodyHtml = true,
            };
            mailMessage.To.Add("edufraeisk@gmail.com");

            smtpClient.Send(mailMessage);

            alert.Text = "Wortspende erfolgreich versendet. Vielen Dank!";
            alert.Visible = true;
        }
    }
}