using System;

namespace Assets.Scripts
{
    public interface IGenericTrial
    {
        Boolean Correct(object subjectAnswer);
        string ToCSV();
    }
}