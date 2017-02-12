using UnityEngine;

public class TextureCharacter {

    public Texture2D tex;
    private static TextureCharacter instance;

    public static TextureCharacter getInstance() {
        if (instance == null)
            instance = new TextureCharacter();
        return instance;
    }

    private TextureCharacter() {

    }
}
