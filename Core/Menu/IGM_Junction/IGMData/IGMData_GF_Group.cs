﻿namespace OpenVIII
{
    public partial class IGM_Junction
    {
        #region Classes

        private class IGMData_GF_Group : IGMData.Group.Base
        {
            #region Constructors

            public IGMData_GF_Group(params Menu_Base[] d) : base(d) => Hide();

            #endregion Constructors
        }

        #endregion Classes
    }
}