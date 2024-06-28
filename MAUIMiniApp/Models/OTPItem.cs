using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MAUIMiniApp.Models
{
    public class OTPItem
    {
        public string OTP { get; set; }
        public string Account { get; set; }

        public int TimerClock
        {
            get
            {
                TimeSpan ts = new DateTime(2024, 6, 28, 19, 0, 0) - DateTime.Now;
                return ts.Seconds;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
