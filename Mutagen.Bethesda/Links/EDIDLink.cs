﻿using Noggog;
using Noggog.Notifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public class EDIDLink<T> : FormIDLink<T>, IEDIDLink<T>
       where T : MajorRecord
    {
        public static readonly RecordType UNLINKED = new RecordType("\0\0\0\0");
        public RecordType? UnlinkedEDID { get; private set; } = UNLINKED;

        public EDIDLink()
            : base()
        {
            this.Subscribe(UpdateUnlinked);
        }

        public EDIDLink(RecordType unlinkedEDID)
            : this()
        {
            this.UnlinkedEDID = unlinkedEDID;
            this.Subscribe(UpdateUnlinked);
        }

        public EDIDLink(RawFormID unlinkedForm)
            : base(unlinkedForm)
        {
            this.Subscribe(UpdateUnlinked);
        }

        private void UpdateUnlinked(Change<T> change)
        {
            var edid = change.New?.EditorID;
            if (edid != null)
            {
                this.UnlinkedEDID = null;
            }
            else
            {
                this.UnlinkedEDID = UNLINKED;
            }
        }

        public void SetIfSucceeded(TryGet<RecordType> item)
        {
            if (item.Failed) return;
            this.UnlinkedEDID = item.Value;
        }

        public void SetIfSuccessful(TryGet<string> item)
        {
            if (!item.Succeeded) return;
            this.UnlinkedEDID = new RecordType(item.Value);
        }
    }
}
