using KeePass;
using KeePass.Forms;
using KeePass.Plugins;
using KeePass.UI;
using KeePassLib;
using KeePassLib.Collections;
using KeePassLib.Security;
using System;
using System.Linq;
using System.Net;

namespace KeeAnonAddy
{
    public class KeeAnonAddyExt : Plugin
    {
        private Func<string>? accessTokenFactory = null;
        private readonly AnonAddyConfig config = new AnonAddyConfig();

        public override bool Initialize(IPluginHost? host)
        {
            if (host == null)
            {
                return false;
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            GlobalWindowManager.WindowAdded += OnWindowAppeared;
            Program.MainForm.FileOpened += OnDatabaseOpened;

            return true;
        }

        private void OnDatabaseOpened(object sender, FileOpenedEventArgs eventArgs)
        {
            ProtectedString? accessToken = FindAccessTokenIn(eventArgs.Database);

            if (accessToken != null)
            {
                this.accessTokenFactory = () => accessToken.ReadString();
            }
        }

        private ProtectedString? FindAccessTokenIn(PwDatabase database)
        {
            var results = new PwObjectList<PwEntry>();

            database.RootGroup.SearchEntries(new SearchParameters
            {
                SearchInStringNames = true,
                ComparisonMode = StringComparison.InvariantCultureIgnoreCase,
                SearchString = this.config.AccessTokenStringKey
            }, results);

            PwEntry? entry = results.FirstOrDefault(e => e.Strings.Exists(this.config.AccessTokenStringKey));

            return entry?.Strings.Get(this.config.AccessTokenStringKey);
        }

        private void OnWindowAppeared(object sender, GwmWindowEventArgs eventArgs)
        {
            if (eventArgs.Form is not PwEntryForm pwEntryForm)
            {
                return;
            }

            var formUtil = new EntryFormUtil(pwEntryForm);

            var menuItem = new CreateAddressMenuItem();

            menuItem.OnClick(async () =>
            {
                string? description = formUtil.GetTitle();

                var service = new AnonAddyService(this.accessTokenFactory);

                AnonAddyEntry anonAddyEntry = await service.RequestAddress(description);

                formUtil.PopulateUserNameFieldWith(anonAddyEntry);
                formUtil.AddIdFrom(anonAddyEntry);
            });

            if (this.accessTokenFactory != null)
            {
                formUtil.Add(menuItem);
            }
        }
    }
}
