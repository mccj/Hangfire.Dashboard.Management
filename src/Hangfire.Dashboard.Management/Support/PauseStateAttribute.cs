﻿// This file is part of Hangfire.
// Copyright © 2013-2014 Sergey Odinokov.
//
// Hangfire is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation, either version 3
// of the License, or any later version.
//
// Hangfire is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with Hangfire. If not, see <http://www.gnu.org/licenses/>.

using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using System;

namespace Hangfire.Dashboard.Management
{
    public sealed class PauseStateAttribute : JobFilterAttribute, IClientFilter, IServerFilter
    {
        public void OnCreating(CreatingContext filterContext)
        {
            if (filterContext == null) throw new ArgumentNullException(nameof(filterContext));
            var recurringJobId = filterContext.GetJobParameter<string>("RecurringJobId");
            if (!string.IsNullOrWhiteSpace(recurringJobId))
            {
                var recurringJob = filterContext.Connection.GetAllEntriesFromHash("recurring-job:" + recurringJobId);
                var isPauseState = recurringJob.ContainsKey("PauseState") ? SerializationHelper.Deserialize<bool>(recurringJob["PauseState"]) : false;

                if (isPauseState)
                {
                    filterContext.SetJobParameter("PauseState", isPauseState);
                    filterContext.Canceled = true;
                }
            }
        }

        public void OnCreated(CreatedContext filterContext)
        {
        }

        public void OnPerforming(PerformingContext filterContext)
        {
            var isPauseState = filterContext.GetJobParameter<bool>("PauseState");
            if (isPauseState)
            {
                filterContext.Items["PauseState"] = isPauseState;
                filterContext.Canceled = true;
            }
            //var uiCultureName = filterContext.GetJobParameter<string>("CurrentUICulture");

            //if (!String.IsNullOrEmpty(cultureName))
            //{
            //    filterContext.Items["PreviousCulture"] = CultureInfo.CurrentCulture;
            //    SetCurrentCulture(new CultureInfo(cultureName));
            //}

            //if (!String.IsNullOrEmpty(uiCultureName))
            //{
            //    filterContext.Items["PreviousUICulture"] = CultureInfo.CurrentUICulture;
            //    SetCurrentUICulture(new CultureInfo(uiCultureName));
            //}
        }

        public void OnPerformed(PerformedContext filterContext)
        {
            //if (filterContext == null) throw new ArgumentNullException(nameof(filterContext));

            //if (filterContext.Items.ContainsKey("PreviousCulture"))
            //{
            //    SetCurrentCulture((CultureInfo) filterContext.Items["PreviousCulture"]);
            //}
            //if (filterContext.Items.ContainsKey("PreviousUICulture"))
            //{
            //    SetCurrentUICulture((CultureInfo)filterContext.Items["PreviousUICulture"]);
            //}
        }

        //        private static void SetCurrentCulture(CultureInfo value)
        //        {
        //#if NETFULL
        //            System.Threading.Thread.CurrentThread.CurrentCulture = value;
        //#else
        //            CultureInfo.CurrentCulture = value;
        //#endif
        //        }

        //        // ReSharper disable once InconsistentNaming
        //        private static void SetCurrentUICulture(CultureInfo value)
        //        {
        //#if NETFULL
        //            System.Threading.Thread.CurrentThread.CurrentUICulture = value;
        //#else
        //            CultureInfo.CurrentUICulture = value;
        //#endif
        //        }
    }
}