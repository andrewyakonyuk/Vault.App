using System;

namespace Vault.Domain.Models
{
    /// <summary>
    /// An event happening at a certain time and location, such as a concert, lecture, or festival.
    /// Ticketing information may be added via the 'offers' property.
    /// Repeated events may be structured as separate Event objects.
    /// </summary>
    public class Event : Thing
    {
        /// <summary>
        /// The duration of the item
        /// </summary>
        public virtual TimeSpan Duration { get; set; }

        /// <summary>
        /// The start date and time of the item
        /// </summary>
        public virtual DateTime StartDate { get; set; }

        /// <summary>
        /// The end date and time of the item
        /// </summary>
        public virtual DateTime EndDate { get; set; }

        /// <summary>
        /// An eventStatus of an event represents its status;
        /// particularly useful when an event is cancelled or rescheduled.
        /// </summary>
        public virtual EventStatusType StatusType { get; set; }

        /// <summary>
        /// The location of for example where the event is happening
        /// </summary>
        public virtual Place Location { get; set; }

        /// <summary>
        /// Used in conjunction with eventStatus for rescheduled or cancelled events.
        /// This property contains the previously scheduled start date.
        /// For rescheduled events, the startDate property should be used for the newly scheduled start date.
        /// In the (rare) case of an event that has been postponed and rescheduled multiple times,
        /// this field may be repeated.
        /// </summary>
        public virtual DateTime PreviousStartDate { get; set; }
    }

    public enum EventStatusType
    {
        /// <summary>
        /// The event is taking place or has taken place on the startDate as scheduled.
        /// Use of this value is optional, as it is assumed by default.
        /// </summary>
        EventScheduled = 0,

        /// <summary>
        /// The event has been cancelled.
        /// </summary>
        EventCancelled,

        /// <summary>
        /// The event has been postponed and no new date has been set.
        /// The event's previousStartDate should be set.
        /// </summary>
        EventPostphoned,

        /// <summary>
        /// The event has been rescheduled.
        /// The event's previousStartDate should be set to the old date and the startDate should be set to the event's new date.
        /// </summary>
        EventRescheduled
    }
}