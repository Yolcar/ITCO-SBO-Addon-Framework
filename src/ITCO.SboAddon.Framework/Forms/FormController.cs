﻿using ITCO.SboAddon.Framework.Helpers;
using SAPbouiCOM;
using System;

namespace ITCO.SboAddon.Framework.Forms
{
    /// <summary>
    /// Base Form Controller
    /// </summary>
    public abstract class FormController
    {
        private IForm _form;

        /// <summary>
        /// Form Object
        /// </summary>
        public IForm Form
        {
            get
            {
                try
                {
                    var dummy = _form?.VisibleEx;
                }
                catch
                {
                    _form = null;
                }
                return _form;
            }
            set
            {
                _form = value;
            }
        }

        /// <summary>
        /// Embeded resources name
        /// </summary>
        /// <example>Forms.MyForm.srf</example>
        /// <remarks>In VB Embeded Resources does not have Folder added in Name</remarks>
        public virtual string FormResource => $"Forms.{GetType().Name.Replace("Controller", string.Empty)}.srf";

        /// <summary>
        /// Eg. NS_MyFormType1
        /// </summary>
        public virtual string FormType => $"ITCO_{GetType().Name.Replace("Controller", string.Empty)}";

        /// <summary>
        /// Open only once
        /// </summary>
        public virtual bool Unique => true;

        /// <summary>
        /// Create new Form
        /// </summary>        
        public FormController(bool autoStart = false)
        {
            if (autoStart)
                Start();
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~FormController()
        {
            SboApp.Logger.Debug("FormController.Destruct");
        }

        /// <summary>
        /// Initalize and show form
        /// </summary>
        public void Start(BoFormModality modality = BoFormModality.fm_None, BoFormBorderStyle borderStyle = BoFormBorderStyle.fbs_Fixed)
        {
            if (Form != null)
            {
                Form.Select();
                return;
            }
            
            try
            {
                var assembly = GetType().Assembly;
                Form = FormHelper.CreateFormFromResource(FormResource, FormType, null, assembly, modality, borderStyle;
                SboApp.Logger.Debug($"Form created: Type={Form.Type}, UID={Form.UniqueID}");

                try
                {
                    FormCreated();
                }
                catch (Exception e)
                {
                    SboApp.Application.MessageBox($"FormCreated Error: {e.Message}");
                }

                try
                {
                    BindFormEvents();
                }
                catch (Exception e)
                {
                    SboApp.Application.MessageBox($"BindFormEvents Error: {e.Message}");
                }

                Form.Visible = true;
            }
            catch (Exception e)
            {
                SboApp.Application.MessageBox($"Failed to open form {FormType}: {e.Message}");
            }
        }

        /// <summary>
        /// Close Form
        /// </summary>
        public void Close()
        {
            Form.Close();
            Form = null;
        }

        /// <summary>
        /// Edit Form after it is created
        /// </summary>
        public virtual void FormCreated()
        {
        }

        /// <summary>
        /// Bind Events
        /// </summary>
        public virtual void BindFormEvents()
        {
        }

        #region Default IFormMenuItem
        /// <summary>
        /// Menu Id
        /// </summary>
        public string MenuItemId => $"{FormType}_M";

        /// <summary>
        /// Menu position, -1 = Last
        /// </summary>
        public int MenuItemPosition => -1;
        
        #endregion
    }
}
