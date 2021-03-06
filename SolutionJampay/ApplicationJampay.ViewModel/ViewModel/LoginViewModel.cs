﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Input;
using ApplicationJampay.Model.DAL.Usager;
using ApplicationJampay.Model.DAL.Utilisateur;
using ApplicationJampay.Model.Entity;
using ApplicationJampay.Model.Service;
using ApplicationJampay.Model.Service.Dialog;
using ApplicationJampay.Model.Service.SmartCardReader;
using ApplicationJampay.ViewModel.Command;
using GalaSoft.MvvmLight.Messaging;

namespace ApplicationJampay.ViewModel.ViewModel
{
    public class LoginViewModel : IViewModelBase
    {
        #region PropertyChanged stuff
        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public Action Close;

        private int Try;

        /// <summary>
        /// Login Command
        /// </summary>
        private readonly RelayCommand _loginCommand;
        public ICommand LoginCommand => _loginCommand;

        private readonly RelayCommand _cardLoginCommand;
        public ICommand CardLoginCommand => _cardLoginCommand;

        /// <summary>
        /// Logic to access to the Users data
        /// </summary>
        private UtilisateurBusiness _utilisateurBusiness;
        /// <summary>
        /// Constructor
        /// </summary>
        public LoginViewModel()
        {
            _loginCommand = new RelayCommand(() => ConnectViaCredentials(), o => true);
            _cardLoginCommand = new RelayCommand(() => ConnectViaCardReader(), o => true);

            try
            {
                _utilisateurBusiness = new UtilisateurBusiness();
            }
            catch (Exception ex)
            {
                DialogService.ShowErrorWindow(ex.Message);
                Environment.Exit(1);
            }          
        }
        
        #region Properties
        private string _matricule;
        public string Matricule
        {
            get { return _matricule; }
            set
            {
                _matricule = value;
                OnPropertyChanged(nameof(Matricule));
            }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        private SecureString _password;
        public SecureString Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
        #endregion

        /// <summary>
        /// Convert a SecureString into a SHA256 hash
        /// </summary>
        /// <param name="secureString"></param>
        /// <returns></returns>
        private string SecureStringToSHA256(SecureString secureString)
        {
            IntPtr intPtr = IntPtr.Zero;

            try
            {
                intPtr = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                string clearPassword = Marshal.PtrToStringUni(intPtr);

                using(SHA256 hash = SHA256Managed.Create())
                {
                    Encoding encoding = Encoding.UTF8;
                    StringBuilder finalHash = new StringBuilder();

                    byte[] result = hash.ComputeHash(encoding.GetBytes(clearPassword));

                    foreach (byte theByte in result)
                    {
                        finalHash.Append(theByte.ToString("x2"));
                    }

                    return finalHash.ToString();
                }
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(intPtr);
            }
        }

        private void Login(Utilisateur utilisateur)
        {
            try
            {        
                
                switch (utilisateur.Fonction)
                {
                    case "Gérant":
                        DialogGerant.ShowGerantMainView();
                        Close();
                        break;

                    case "Caissier":
                        DialogCaissier.ShowCaissierMainWindow();
                        Messenger.Default.Send<Utilisateur>(utilisateur);
                        Close();
                        break;

                    case "Cuisinier":
                        DialogCuisinier.ShowCuisinierWindow();
                        Close();
                        break;
                }
                
            }
            catch (Exception ex)
            {
                Try++;
                var msg = "Il vous reste " + (5 - Try).ToString() + " essais";
                DialogService.ShowErrorWindow(ex.Message + "\n" + msg);
            }
            
        }

        private void ConnectViaCredentials()
        {
            try
            {
                Utilisateur utilisateur = _utilisateurBusiness.GetUtilisateur(Matricule, SecureStringToSHA256(Password));
                Login(utilisateur);
            }
            catch (Exception)
            {

                DialogService.ShowErrorWindow("Veuillez renseigner correctement les champs");
            }

        }




        private void ConnectViaCardReader()
        {

            try
            {
                Utilisateur utilisateur = _utilisateurBusiness.GetUtilisateurByMatriculeCarte(ManageDataCardService.GetCodeCarte());
                Login(utilisateur);
            }
            catch (Exception ex)
            {

                DialogService.ShowErrorWindow(ex.Message);
            }
        }
                    
    }
}
