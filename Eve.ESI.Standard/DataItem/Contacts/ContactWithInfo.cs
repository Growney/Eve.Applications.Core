namespace Eve.ESI.Standard.DataItem.Contacts
{
    public class ContactWithInfo<ContactType,InfoType> where ContactType : Contact
    {
        private ContactType m_contact;
        private InfoType m_character;

        public ContactWithInfo(ContactType contact, InfoType character)
        {
            m_contact = contact;
            m_character = character;
        }
    }
}
