using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfRaziLedgerApp
{
    public class MyPublisher
    {
        // تعریف event با پارامترهای دلخواه
        public event EventHandler<EventNav> MyEvent;
        public EventNav eventNav;

        public void DoSomething()
        {

            // وقتی رویداد اجرا میشه، اطلاعات دلخواه ارسال می‌کنیم
            OnMyEvent(eventNav);
        }

        // متد محافظت‌شده برای صدا زدن event
        protected virtual void OnMyEvent(EventNav e)
        {
            MyEvent?.Invoke(this, e); // اول sender (این کلاس) بعد پارامترها
        }
    }
}
