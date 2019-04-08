public class ImageAction {

    public int Pixel {
        get;
        private set;
    }
    
    public enum ActionType{
        Push, Pop, Check
    }

    public ActionType Type { get; private set; }

    public ImageAction(int pixel, ActionType type) {
        Pixel = pixel;
        Type = type;
    }

    public override bool Equals(object obj) {
        if (obj.GetType() != typeof(ImageAction)) return false;
        ImageAction other = (ImageAction)obj;
        return Pixel == other.Pixel && Type == other.Type;
    }

    public override int GetHashCode() {
        var hashCode = 1190742396;
        hashCode = hashCode * -1521134295 + Pixel.GetHashCode();
        hashCode = hashCode * -1521134295 + Type.GetHashCode();
        return hashCode;
    }
}