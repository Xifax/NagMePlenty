using System;
using System.Collections.Generic;
using System.IO;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace NagMePlenty
{
    class Toaster
    {
        private String APP_ID;
        private ToastNotifier notifier;

        public Toaster(String APP_ID)
        {
            this.APP_ID = APP_ID;
            notifier = ToastNotificationManager.CreateToastNotifier(APP_ID);
        }

        public ToastNotification showToast(List<String> items = null) {

            /*
            String xml =
                @"<toast>
                    <visual>
                        <binding template='ToastImageAndText02'>
                            <image id='1' src='{0}' />
                            <text id='1'>{1}</text>
                            <text id='2'>{2}</text>
                            <text id='3'>{3}</text>
                        </binding>
                    </visual>
                </toast>";

            XmlDocument customXml  = new XmlDocument();
            customXml.LoadXml(xml);
            */

            // Get a toast XML template
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText04);

            // Fill in the text elements
            XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
            for (int i = 0; i < stringElements.Length; i++)
            {
                stringElements[i].AppendChild(toastXml.CreateTextNode(items[i]));
            }

            // Specify the absolute path to an image
            String imagePath = "file:///" + Path.GetFullPath("Resources/Reminder.png");
            XmlNodeList imageElements = toastXml.GetElementsByTagName("image");
            imageElements[0].Attributes.GetNamedItem("src").NodeValue = imagePath;

            // Create the toast and attach event listeners
            ToastNotification toast = new ToastNotification(toastXml);

            // Show the toast. Be sure to specify the AppUserModelId on your application's shortcut!
            notifier.Show(toast);
            return toast;
        }


        /*
        private void toastactivated(toastnotification sender, object e)
        {
            dispatcher.invoke(() =>
            {
                activate();
                output.text = "the user activated the toast.";
            });
        }

        private void toastdismissed(toastnotification sender, toastdismissedeventargs e)
        {
            string outputtext = "";
            switch (e.reason)
            {
                case toastdismissalreason.applicationhidden:
                    outputtext = "the app hid the toast using toastnotifier.hide";
                    break;
                case toastdismissalreason.usercanceled:
                    outputtext = "the user dismissed the toast";
                    break;
                case toastdismissalreason.timedout:
                    outputtext = "the toast has timed out";
                    break;
            }

            dispatcher.invoke(() =>
            {
                output.text = outputtext;
            });
        }

        private void ToastFailed(ToastNotification sender, ToastFailedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                Output.Text = "The toast encountered an error.";
            });
        }
        */
    }
}
