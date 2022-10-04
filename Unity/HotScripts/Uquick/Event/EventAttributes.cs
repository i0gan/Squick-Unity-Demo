
using System;
namespace Uquick.Event
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class SubscriberAttribute : Attribute
    {
        private ThreadMode _threadMode = ThreadMode.Main;
        public ThreadMode ThreadMode => _threadMode;

        public SubscriberAttribute()
        {
            this._threadMode = ThreadMode.Main;
        }

        public SubscriberAttribute(ThreadMode ThreadMode = ThreadMode.Main)
        {
            this._threadMode = ThreadMode;
        }
    }
}