﻿using System;
using Toggl.Foundation.Models.Interfaces;

namespace Toggl.Foundation.Calendar
{
    public struct CalendarItem
    {
        public CalendarItemSource Source { get; }

        public TimeSpan Duration { get; }

        public DateTimeOffset StartTime { get; }

        public DateTimeOffset EndTime => StartTime + Duration;

        public string Description { get; }

        public string Color { get; }

        public long? TimeEntryId { get; }

        public CalendarItem(CalendarItemSource source,
            DateTimeOffset startTime,
            TimeSpan duration,
            string description,
            string color = "",
            long? timeEntryId = null)
        {
            Source = source;
            StartTime = startTime;
            Duration = duration;
            Description = description;
            Color = color;
            TimeEntryId = timeEntryId;
        }

        public CalendarItem(IThreadSafeTimeEntry timeEntry)
            : this(CalendarItemSource.TimeEntry,
                timeEntry.Start,
                TimeSpan.FromSeconds(timeEntry.Duration.Value),
                timeEntry.Description,
                timeEntry.Project.Color,
                timeEntry.Id)
        {
        }
    }
}