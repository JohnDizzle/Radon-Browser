using System;

namespace Yttrium_browser.ViewModels.MessageRecords
{
    public record class MessageStatus(string v, Uri source = null)
    {

        public MessageStatus(string v) : this(v, null)
        {
            V = v;
        }

        public string V { get; }
    }

}
