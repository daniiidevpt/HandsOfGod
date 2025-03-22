namespace HOG.Resources
{
    public struct ResourceCollectionInfo
    {
        public ResourceType m_ResourceType;
        public int m_Amount;

        public ResourceCollectionInfo(ResourceType resourceType, int amount)
        {
            m_ResourceType = resourceType;
            m_Amount = amount;
        }
    }
}
