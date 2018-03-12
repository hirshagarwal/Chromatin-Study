﻿using UnityEngine;

namespace Assets.Scripts
{
    public interface IObjectManager
    {
        Material BaseMaterial { get; }
        TextAsset TextFile { get; }
        Curve MainCurve { get; }
        Curve RedCurve { get; }
        Curve BlueCurve { get; }
        GameObject BlueObject { get; }
        GameObject RedObject { get; }
        bool ShowUnderstanding { get; }
        string UnderstandingString { get; }

        void SetupCurveComparisonTrial(CurveComparisonTrial curveComparisonTrial);

        void SetupPointDistanceTrial(PointDistanceTrial pdt, string chrfn);

        void SetupAttributeUnderstandingTrial(AttributeUnderstandingTrial adt);

        void SetupSegmentDistanceTrial(SegmentDistanceTrial sdt, string chrfn);

        void SetupTouchingSegments(TouchingPointsTrial tst);

        void SetupLargerTadTrial(LargerTadTrial ltt);

        void LoadNextFile(string filename);
    }
}