#region Copyright
// <copyright file="MvxBaseConsoleSetup.cs" company="Cirrious">
// (c) Copyright Cirrious. http://www.cirrious.com
// This source is subject to the Microsoft Public License (Ms-PL)
// Please see license.txt on http://opensource.org/licenses/ms-pl.html
// All other rights reserved.
// </copyright>
// 
// Author - Stuart Lodge, Cirrious. http://www.cirrious.com
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Cirrious.MvvmCross.Console.Interfaces;
using Cirrious.MvvmCross.Console.Views;
using Cirrious.MvvmCross.ExtensionMethods;
using Cirrious.MvvmCross.Interfaces.Platform.Diagnostics;
using Cirrious.MvvmCross.Interfaces.Plugins;
using Cirrious.MvvmCross.Interfaces.ServiceProvider;
using Cirrious.MvvmCross.Interfaces.Views;
using Cirrious.MvvmCross.Platform;
using Cirrious.MvvmCross.Platform.Diagnostics;
using Cirrious.MvvmCross.Plugins;
using Cirrious.MvvmCross.Views;

namespace Cirrious.MvvmCross.Console.Platform
{
    public class MvxConsolePluginManager : MvxFileBasedPluginManager
    {
        public MvxConsolePluginManager()
            : base("Console")
        {
        }

        protected override Assembly LoadAssembly(Type toLoad)
        {
            MvxTrace.Trace("Loading plugin for {0}", toLoad.AssemblyQualifiedName);
            var fileName = GetPluginAssemblyNameFrom(toLoad);
            MvxTrace.Trace("- plugin assembly is {0}", fileName);
            var assembly = Assembly.LoadFrom(fileName);
            return assembly;
        }

        protected override string GetPluginAssemblyNameFrom(Type toLoad)
        {
            var fileName = base.GetPluginAssemblyNameFrom(toLoad);
            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(location, fileName); 
        }
    }

    public abstract class MvxBaseConsoleSetup 
        : MvxBaseSetup        
        , IMvxServiceProducer<IMvxConsoleCurrentView>
        , IMvxServiceProducer<IMvxMessagePump>
        , IMvxServiceProducer<IMvxConsoleNavigation>
        , IMvxServiceProducer<IMvxTrace>
    {
        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void InitializeDebugServices()
        {
            this.RegisterServiceInstance<IMvxTrace>(new MvxDebugTrace());
            base.InitializeDebugServices();
        }

        public virtual void InitializeMessagePump()
        {
            var messagePump = new MvxConsoleMessagePump();
            this.RegisterServiceInstance<IMvxMessagePump>(messagePump);
            this.RegisterServiceInstance<IMvxConsoleCurrentView>(messagePump);
        }

        protected override MvxViewsContainer CreateViewsContainer()
        {
            var container = CreateConsoleContainer();
            this.RegisterServiceInstance<IMvxConsoleNavigation>(container);
            return container;
        }

        protected override IMvxViewDispatcherProvider CreateViewDispatcherProvider()
        {
            return new MvxConsoleDispatcherProvider();
        }

        protected virtual MvxBaseConsoleContainer CreateConsoleContainer()
        {
            return new MvxConsoleContainer();
        }

        protected override void InitializeLastChance()
        {
            InitializeMessagePump();
        }

        protected override IMvxPluginManager CreatePluginManager()
        {
            return new MvxConsolePluginManager();
        }

        protected override IDictionary<System.Type, System.Type> GetViewModelViewLookup()
        {
            return GetViewModelViewLookup(GetType().Assembly, typeof(IMvxConsoleView));
        }
    }
}