﻿using System.Collections.Generic;

namespace Toggl.Foundation.Autocomplete.Suggestions
{
    public sealed class AutocompleteSuggestionComparer : IEqualityComparer<AutocompleteSuggestion>
    {
        public static AutocompleteSuggestionComparer Instance { get; } = new AutocompleteSuggestionComparer();

        private AutocompleteSuggestionComparer() { }

        public bool Equals(AutocompleteSuggestion x, AutocompleteSuggestion y)
        {
            switch (x)
            {
                case TimeEntrySuggestion teX:
                    return y is TimeEntrySuggestion teY
                        && teX.Description == teY.Description && teX.ProjectId == teY.ProjectId;

                case ProjectSuggestion pX:
                    return y is ProjectSuggestion pY && pX.ProjectId == pY.ProjectId;

                case TagSuggestion tX:
                    return y is TagSuggestion tY && tX.Name == tY.Name;
            }

            return x == y;
        }

        public int GetHashCode(AutocompleteSuggestion suggestion)
            => suggestion.GetHashCode();
    }
}
