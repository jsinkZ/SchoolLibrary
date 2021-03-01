using System;

namespace SchoolLibrary3.ViewModels.Home
{
    public class PrivacyViewModel
    {
        public PrivacyViewModel(AgreementType type)
        {
            switch (type)
            {
                case AgreementType.GDPR:
                    header = "Политика конфиденциальности сookie";
                    target = String.Empty;
                    list = String.Empty;
                    break;
                case AgreementType.Client:
                    header = "Согласие на обработку персональных данных Пользователя информационной системы «Школьная библиотека»";
                    target = "идентификация стороны при регистрации в информационной системе";
                    list = "фамилия, имя, отчество, дата рождения, пол, класс, адрес электронной почты, телефон";
                    break;
                case AgreementType.Admin:
                    header = "Согласие на обработку персональных данных Администратора  информационной системы «Школьная библиотека»";
                    target = "управление информационной системой";
                    list = "адрес электронной почты и телефон";
                    break;
                case AgreementType.Librarian:
                    header = "Согласие на обработку персональных данных Библиотекаря";
                    target = "учёта данных о движении библиотечного фонда";
                    list = "фамилия, имя, отчество";
                    break;
            }
        }

        private String header;
        public String HeaderMessage
        {
            get { return header; }
        }
        private String target;
        public String TargetMessage
        {
            get { return target; }
        }
        private String list;
        public String ListMessage
        {
            get { return list; }
        }
    }

    public enum AgreementType
    {
        GDPR, Client, Admin, Librarian
    }
}
