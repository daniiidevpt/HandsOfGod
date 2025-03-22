using HOG.Resources;

public interface ICollectable
{
    public ResourceCollectionInfo Collect();
    public void TakeHit(int hitAmount);
}


