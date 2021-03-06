﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

namespace Microsoft.Azure.Commands.Automation.Model
{
    using Microsoft.Azure.Commands.Automation.Common;
    using Microsoft.Azure.Commands.Automation.Properties;
    using System;
    using System.Globalization;

    /// <summary>
    /// The hourly schedule.
    /// </summary>
    public class HourlySchedule : Schedule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HourlySchedule"/> class.
        /// </summary>
        /// <param name="schedule">
        /// The schedule.
        /// </param>
        public HourlySchedule(Azure.Management.Automation.Models.Schedule schedule)
        {
            Requires.Argument("schedule", schedule).NotNull();

            if (!schedule.HourInterval.HasValue)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidHourlyScheduleModel, schedule.Name));
            }

            this.Id = new Guid(schedule.Id);
            this.AccountId = new Guid(schedule.AccountId);
            this.Name = schedule.Name;
            this.Description = schedule.Description;
            this.StartTime = DateTime.SpecifyKind(schedule.StartTime, DateTimeKind.Utc).ToLocalTime();
            this.ExpiryTime = DateTime.SpecifyKind(schedule.ExpiryTime, DateTimeKind.Utc).ToLocalTime();
            this.CreationTime = DateTime.SpecifyKind(schedule.CreationTime, DateTimeKind.Utc).ToLocalTime();
            this.LastModifiedTime = DateTime.SpecifyKind(schedule.LastModifiedTime, DateTimeKind.Utc).ToLocalTime();
            this.IsEnabled = schedule.IsEnabled;
            this.NextRun = schedule.NextRun.HasValue
                               ? DateTime.SpecifyKind(schedule.NextRun.Value, DateTimeKind.Utc).ToLocalTime()
                               : this.NextRun;
            this.HourInterval = schedule.HourInterval.Value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HourlySchedule"/> class.
        /// </summary>
        public HourlySchedule()
        {
        }

        /// <summary>
        /// Gets or sets The next run time of the schedule.
        /// </summary>
        public int HourInterval { get; set; }
    }
}
