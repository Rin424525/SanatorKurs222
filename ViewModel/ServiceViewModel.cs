using Sanator;
using Sanator.ModelDb;
using Sanator.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sanator.ViewModel
{
    public class ServiceViewModel : INotifyPropertyChanged
    {
        public DbOperations db;
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<Service> services { get; set; }
        private Service selectedService;
        private IDialogService ds;

        public string name { get; set; }
        public int pay { get; set; }
        public  string description {get;set;}

        public ServiceViewModel(DbOperations db, IDialogService ds)
        {
            this.ds = ds;
            this.db = db;
            services = new ObservableCollection<Service>(db.GetAllService());
            SelectedService=db.GetAllService().FirstOrDefault();
        }
        public Service SelectedService
        {
            get { return selectedService; }
            set
            {
                selectedService = value;
                OnPropertyChanged("SelectedService");
            }
        }
        private RelayCommand addService;
        public RelayCommand AddService
        {
            get
            {
                return addService ??
                    (addService = new RelayCommand(obj =>
                    {
                        Service service = new Service() { Name = "Новая услуга"};
                        services.Insert(0, service);
                        SelectedService = service;
                        ds.ShowMessage("Новая услуга добавлена!");
                    }));
            }
        }
        private RelayCommand removeService;
        public RelayCommand RemoveService
        {
            get
            {
                return removeService ??
                    (removeService = new RelayCommand(obj =>
                    {

                        if (db.FindService(SelectedService.ID_service) != null)
                        {
                            db.RemoveService(SelectedService);
                            services.Remove(SelectedService);
                            db.Save();
                            ds.ShowMessage("Объект удален!");
                        }
                        else services.Remove(SelectedService);
                    },
                    (obj) => SelectedService != null));
            }
        }

        private RelayCommand saveService;
        public RelayCommand SaveService
        {
            get
            {
                return saveService ??
                    (saveService = new RelayCommand(obj =>
                    {
                        if (SelectedService != null)
                        {
                            var service = db.FindService(SelectedService.ID_service);
                            if (service == null)
                            {
                                db.AddService(SelectedService);
                            }
                            db.Save();
                            ds.ShowMessage("Изменения сохранены!");
                        }

                    },
                    (obj) => (SelectedService != null) && SelectedService.Name.TrimEnd() != "Новая услуга" && SelectedService.Description != null));
            }
        }
        private bool CanExecuteSave()
        {
            if (SelectedService != null)
            {
                if (SelectedService.description.TrimEnd() != "Новый клиент" && SelectedService.name != null && SelectedService.pay != null) return true;
            }
            return false;

        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        void RemoveClientById(int id)
        {
            MySqlDB.GetDB().ExecuteNonQuery("delete from Client where ID_client = " + id);
        }
    }
}
