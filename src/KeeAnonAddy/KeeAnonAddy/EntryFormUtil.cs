﻿using KeePass.Forms;
using KeePass.Plugins;
using KeePassLib.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeeAnonAddy
{
    public class EntryFormUtil
    {
        public readonly PwEntryForm form;

        public EntryFormUtil(PwEntryForm form)
        {
            this.form = form;
        }

        internal void Add(CreateAddressMenuItem menuItem)
        {
            var item = menuItem.ToToolStripItem();
            AddMenuItemAfter("m_ctxToolsUrlSelDoc", item);
        }

        private void AddMenuItemAfter(string key, ToolStripItem item)
        {
            int ctxToolsUrlSelDocIndex = form.ToolsContextMenu.Items.IndexOfKey(key);

            if (ctxToolsUrlSelDocIndex >= 0)
            {
                form.ToolsContextMenu.Items.Insert(ctxToolsUrlSelDocIndex + 1, item);
            }
            else
            {
                form.ToolsContextMenu.Items.Add(item);
            }
        }

        internal string GetTitle()
        {
            var titleControl = GetControlByKey("m_tbTitle");
            return titleControl?.Text;
        }

        private Control GetControlByKey(string controlKey)
        {
            Control[] controls = this.form.Controls.Find(controlKey, true);

            return controls.FirstOrDefault();
        }

        internal void PopulateUserNameFieldWith(AnonAddyEntry anonAddyEntry)
        {
            var userNameControl = GetControlByKey("m_tbUserName");
            if (userNameControl != null)
            {
                userNameControl.Text = anonAddyEntry.MailAddress.Address;
            }

            form.EntryRef.Touch(true, false);
        }

        internal void AddIdFrom(AnonAddyEntry anonAddyEntry)
        {
            if (anonAddyEntry.Id != null)
            {
                var protectedId = new ProtectedString(false, anonAddyEntry.Id.ToString());
                form.EntryStrings.Set("x-aa-id", protectedId);
            }

            form.EntryRef.Touch(true, false);
        }
    }
}