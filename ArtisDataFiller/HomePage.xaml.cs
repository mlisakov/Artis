using System.Net;
using System.Net.Mail;
using System.Windows;

namespace Artis.ArtisDataFiller
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var fromAddress = new MailAddress("ОТПРАВИТЕЛЬ @mail.ru", "ИМЯ ОТПРАВИТЕЛЯ");
            var toAddress = new MailAddress("ПОЛУЧАТЕЛЬ @mail.ru", "ИМЯ ПОЛУЧАТЕЛЯ");

            const string fromPassword = "ПАРОЛЬ ОТПРАВИТЕЛЯ";
            const string subject = "ТЕМА ПИСЬМА";
            const string body =
                "Уважаемый Максимус! Я ненавижу нас двоих за то, что мы в это ввязались! Кстати говоря - отправка мыла не проблема." +
                "С уважением, " +
                "Ваша тулза администрирования.";

            //todo smtp клиент должен соответствовать тому почтовому серверу, на котором зареган отправитель.
            var smtp = new SmtpClient
            {
                Host = "smtp.mail.ru",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod =  SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }
    }
}
