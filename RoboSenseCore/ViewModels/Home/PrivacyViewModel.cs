using System;

namespace RoboSenseCore.ViewModels.Home
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
                case AgreementType.Deputy:
                    header = "Согласие на обработку персональных данных Представителя образовательного учреждения или семьи";
                    target = "идентификация стороны при регистрации на Фестиваль";
                    list = "принадлежность к образовательному учреждению или семье, адрес электронной почты, телефон";
                    break;
                case AgreementType.Tutor:
                    header = "Согласие на обработку персональных данных Руководителя команды";
                    target = "регистрация участников Фестиваля";
                    list = "фамилия, имя, отчество, дата рождения, пол, должность/позиция в семье, адрес электронной почты и телефон";
                    break;
                case AgreementType.Challenger:
                    header = "Согласие на обработку персональных данных Участника команды";
                    target = "регистрация участников Фестиваля";
                    list = "фамилия, имя, отчество, дата рождения, пол, принадлежность к образовательному учреждению, класс";
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
        GDPR, Deputy, Tutor, Challenger
    }
}
