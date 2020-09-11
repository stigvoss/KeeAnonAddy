using KeePass;
using KeePass.Forms;
using KeePass.Plugins;
using KeePass.UI;
using KeePassLib;
using KeePassLib.Collections;
using KeePassLib.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeeAnonAddy
{
    public class KeeAnonAddyExt : Plugin
    {
        private Func<string> accessTokenFactory;
        private IPluginHost host;

        public override bool Initialize(IPluginHost host)
        {
            if (host == null)
            {
                return false;
            }

            this.host = host;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            GlobalWindowManager.WindowAdded += OnWindowAppeared;
            Program.MainForm.FileOpened += OnDatabaseOpened;

            return true;
        }

        private void OnDatabaseOpened(object sender, FileOpenedEventArgs eventArgs)
        {
            ProtectedString accessToken = FindAccessTokenIn(eventArgs.Database);

            if (accessToken != null)
            {
                accessTokenFactory = () => accessToken.ReadString();
            }
        }

        private ProtectedString FindAccessTokenIn(PwDatabase database)
        {
            var results = new PwObjectList<PwEntry>();

            database.RootGroup.SearchEntries(new SearchParameters
            {
                SearchInStringNames = true,
                ComparisonMode = StringComparison.InvariantCultureIgnoreCase,
                SearchString = "x-aa-api-token"
            }, results);

            PwEntry entry = results.FirstOrDefault(e => e.Strings.Exists("x-aa-api-token"));

            return entry?.Strings.Get("x-aa-api-token");
        }

        private void OnWindowAppeared(object sender, GwmWindowEventArgs eventArgs)
        {
            if (eventArgs.Form is PwEntryForm pwEntryForm)
            {
                var formUtil = new EntryFormUtil(pwEntryForm);

                var menuItem = new CreateAddressMenuItem();

                menuItem.OnClick(async () =>
                {
                    string description = formUtil.GetTitle();

                    var service = new AnonAddyService(this.accessTokenFactory);

                    AnonAddyEntry anonAddyEntry = await service.RequestAddress(description);

                    formUtil.PopulateUserNameFieldWith(anonAddyEntry);
                    formUtil.AddIdFrom(anonAddyEntry);
                });

                if (accessTokenFactory != null)
                {
                    formUtil.Add(menuItem);
                }
            }
        }
    }
}
