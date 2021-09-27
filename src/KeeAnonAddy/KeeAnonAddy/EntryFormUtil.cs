using KeePass.Forms;
using KeePassLib.Security;
using System.Linq;
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

        internal void Add(IMenuItem menuItem)
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

        internal string? GetTitle()
        {
            Control? titleControl = GetControlByKey("m_tbTitle");
            return titleControl?.Text;
        }

        private Control? GetControlByKey(string controlKey)
        {
            Control[] controls = this.form.Controls.Find(controlKey, true);

            return controls.FirstOrDefault();
        }

        internal void PopulateUserNameFieldWith(AnonAddyEntry anonAddyEntry)
        {
            Control? userNameControl = GetControlByKey("m_tbUserName");
            if (userNameControl != null)
            {
                userNameControl.Text = anonAddyEntry.MailAddress.Address;
            }

            this.form.EntryRef.Touch(true, false);
        }

        internal void AddIdFrom(AnonAddyEntry anonAddyEntry)
        {
            if (anonAddyEntry.Id != null)
            {
                var protectedId = new ProtectedString(false, anonAddyEntry.Id.ToString());
                this.form.EntryStrings.Set("x-aa-id", protectedId);
            }

            this.form.EntryRef.Touch(true, false);
        }
    }
}
