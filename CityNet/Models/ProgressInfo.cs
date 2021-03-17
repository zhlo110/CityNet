using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.Models
{
    public class ProgressInfo
    {
        string mUniqueID = "";

        public string UniqueID
        {
            get { return mUniqueID; }
            set { mUniqueID = value; }
        }
        int mTotal = -1;

        public int Total
        {
            get { return mTotal; }
            set { mTotal = value; }
        }

        string mDescription = "";

        public string Description
        {
            get { return mDescription; }
            set { mDescription = value; }
        }

        int mCurrent = -1;

        public int Current
        {
            get { return mCurrent; }
            set { mCurrent = value; }
        }

        string mText = "";

        public string Text
        {
            get { return mText; }
            set { mText = value; }
        }

    }
}