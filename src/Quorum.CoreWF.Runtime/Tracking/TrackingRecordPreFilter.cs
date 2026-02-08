// This file is part of Core WF which is licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace System.Activities.Tracking;

public class TrackingRecordPreFilter
{
    public TrackingRecordPreFilter() { }

    public TrackingRecordPreFilter(bool trackingProviderInitialized)
    {
        if (trackingProviderInitialized)
        {
            TrackingProviderInitialized = true;
            TrackActivityScheduledRecords = true;
            TrackActivityStateRecords = true;
            TrackActivityStateRecordsClosedState = true;
            TrackActivityStateRecordsExecutingState = true;
            TrackBookmarkResumptionRecords = true;
            TrackCancelRequestedRecords = true;
            TrackFaultPropagationRecords = true;
            TrackWorkflowInstanceRecords = true;
        }
    }

    public bool TrackingProviderInitialized { get; set; }

    public bool TrackWorkflowInstanceRecords { get; set; }

    public bool TrackBookmarkResumptionRecords { get; set; }

    public bool TrackActivityScheduledRecords { get; set; }

    public bool TrackActivityStateRecordsClosedState { get; set; }

    public bool TrackActivityStateRecordsExecutingState { get; set; }

    public bool TrackActivityStateRecords { get; set; }

    public bool TrackCancelRequestedRecords { get; set; }

    public bool TrackFaultPropagationRecords { get; set; }

    public void Merge(TrackingRecordPreFilter filter)
    {
        if (TrackingProviderInitialized)
        {
            TrackingProviderInitialized = false;
            TrackActivityStateRecordsExecutingState = filter.TrackActivityStateRecordsExecutingState;
            TrackActivityScheduledRecords = filter.TrackActivityScheduledRecords;
            TrackActivityStateRecords = filter.TrackActivityStateRecords;
            TrackActivityStateRecordsClosedState = filter.TrackActivityStateRecordsClosedState;
            TrackBookmarkResumptionRecords = filter.TrackBookmarkResumptionRecords;
            TrackCancelRequestedRecords = filter.TrackCancelRequestedRecords;
            TrackFaultPropagationRecords = filter.TrackFaultPropagationRecords;
            TrackWorkflowInstanceRecords = filter.TrackWorkflowInstanceRecords;
        }
        else
        {
            TrackActivityStateRecordsExecutingState |= filter.TrackActivityStateRecordsExecutingState;
            TrackActivityScheduledRecords |= filter.TrackActivityScheduledRecords;
            TrackActivityStateRecords |= filter.TrackActivityStateRecords;
            TrackActivityStateRecordsClosedState |= filter.TrackActivityStateRecordsClosedState;
            TrackBookmarkResumptionRecords |= filter.TrackBookmarkResumptionRecords;
            TrackCancelRequestedRecords |= filter.TrackCancelRequestedRecords;
            TrackFaultPropagationRecords |= filter.TrackFaultPropagationRecords;
            TrackWorkflowInstanceRecords |= filter.TrackWorkflowInstanceRecords;
        }
    }
}
