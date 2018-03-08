﻿using System;

namespace Assets.Scripts
{
    public interface IGenericTrial
    {
        Boolean Correct(object subjectAnswer);

        Formats StudyFormat { get; }

        string ToCSV();
    }
}