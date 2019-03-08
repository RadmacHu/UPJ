
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CreateAssetMenu(menuName = "BMFontConfig")]
public class BMFontConfigData : ScriptableObject
{
    public UIAtlas m_ArtFontAtlas;

    public List<BMFontElement> ArtFontCreateList = new List<BMFontElement>();

    public void SetArtFontParams()
    {
        if(m_ArtFontAtlas == null)
            return;

        if(m_ArtFontAtlas.spriteList == null)
            return;

        if(m_ArtFontAtlas.spriteList.Count != ArtFontCreateList.Count)
            return;

        for (int i = 0; i < ArtFontCreateList.Count; i++)
        {
            ArtFontCreateList[i].m_Name = m_ArtFontAtlas.spriteList[i].name;
            ArtFontCreateList[i].uvX = m_ArtFontAtlas.spriteList[i].x;
            ArtFontCreateList[i].uvY = m_ArtFontAtlas.spriteList[i].y;
            ArtFontCreateList[i].fontTexture = (Texture2D) m_ArtFontAtlas.texture;
            ArtFontCreateList[i].fontMaterial = m_ArtFontAtlas.spriteMaterial;
        }
    }

    public void CreateBMFonts()
    {
        if(ArtFontCreateList == null || ArtFontCreateList.Count <= 0)
            return;

        for (int i = 0; i < ArtFontCreateList.Count; i++)
        {
            ArtFontCreateList[i].CreateSingleBMFont();
        }
    }
}

[System.Serializable]
public class BMFontElement
{
    public string m_Name;
    public float uvX;
    public float uvY;
    public Font targetFont;
    public TextAsset fntData;
    public Material fontMaterial;
    public Texture2D fontTexture;

    public void CreateSingleBMFont()
    {
        BMFontEditor.CreateBMFont(uvX , uvY , targetFont , fntData ,fontMaterial , fontTexture);
    }
}



public class BMFontEditor : EditorWindow
{
    

    [MenuItem("Tools/BMFont Maker")]
    static public void OpenBMFontMaker()
    {
        EditorWindow.GetWindow<BMFontEditor>(false, "BMFont Maker", true).Show();
    }

    public BMFontConfigData bmFontCreateConfig;

    //public float uvX;
    //public float uvY;

    //[SerializeField]
    //private Font targetFont;
    //[SerializeField]
    //private TextAsset fntData;
    //[SerializeField]
    //private Material fontMaterial;
    //[SerializeField]
    //private Texture2D fontTexture;
    //private BMFont bmFont = new BMFont();

    public BMFontEditor()
    {
    }

    void OnGUI()
    {
        bmFontCreateConfig = 
            EditorGUILayout.ObjectField("Art Font Create Config", bmFontCreateConfig, typeof (BMFontConfigData)) as BMFontConfigData;

        if (GUILayout.Button("Init BMFont"))
        {
            if (bmFontCreateConfig != null)
            {
                bmFontCreateConfig.SetArtFontParams();
            }
        }

        if (GUILayout.Button("Create BMFonts"))
        {
            if (bmFontCreateConfig != null)
            {
                bmFontCreateConfig.CreateBMFonts();
            }
        }
        //uvX = EditorGUILayout.FloatField("UV X", uvX);
        //uvY = EditorGUILayout.FloatField("UV Y", uvY);
        //targetFont = EditorGUILayout.ObjectField("Target Font", targetFont, typeof(Font), false) as Font;
        //fntData = EditorGUILayout.ObjectField("Fnt Data", fntData, typeof(TextAsset), false) as TextAsset;
        //fontMaterial = EditorGUILayout.ObjectField("Font Material", fontMaterial, typeof(Material), false) as Material;
        //fontTexture = EditorGUILayout.ObjectField("Font Texture", fontTexture, typeof(Texture2D), false) as Texture2D;
        //if (GUILayout.Button("Create BMFont"))
        //{
        //    BMFontReader.Load(bmFont, fntData.name, fntData.bytes); // 借用NGUI封装的读取类
        //    CharacterInfo[] characterInfo = new CharacterInfo[bmFont.glyphs.Count];
        //    for (int i = 0; i < bmFont.glyphs.Count; i++)
        //    {
        //        BMGlyph bmInfo = bmFont.glyphs[i];
        //        CharacterInfo info = new CharacterInfo();
        //        info.index = bmInfo.index;
        //        //info.uv.x = (float)bmInfo.x / (float)bmFont.texWidth;
        //        //info.uv.y = 1 - (float)bmInfo.y / (float)bmFont.texHeight;
        //        //info.uv.width = (float)(bmInfo.width) / (float)bmFont.texWidth;
        //       // info.uv.height = -1f * (float)(bmInfo.height) / (float)bmFont.texHeight;

        //        info.uv.x = (float)(bmInfo.x + uvX) / (float)fontTexture.width;
        //        info.uv.y = 1 - (float)(bmInfo.y + uvY) / (float)fontTexture.height;
        //        info.uv.width = (float)(bmInfo.width) / (float)fontTexture.width;
        //        info.uv.height = -1f * (float)(bmInfo.height) / (float)fontTexture.height;

        //        info.vert.x = 0;
        //        info.vert.y = -(float)bmInfo.height;
        //        info.vert.width = (float)bmInfo.width;
        //        info.vert.height = (float)bmInfo.height;
        //        info.width = (float)bmInfo.advance;
        //        characterInfo[i] = info;
        //    }
        //    targetFont.characterInfo = characterInfo;
        //    if (fontMaterial)
        //    {
        //        fontMaterial.mainTexture = fontTexture;
        //    }
        //    targetFont.material = fontMaterial;
        //    //fontMaterial.shader = Shader.Find("UI/Default");//这一行很关键，如果用standard的shader，放到Android手机上，第一次加载会很慢

        //    Debug.Log("create font <" + targetFont.name + "> success");
        //    //Close();

        //    EditorUtility.SetDirty(targetFont);
        //    EditorUtility.SetDirty(fontMaterial);
        //    AssetDatabase.SaveAssets();
        //    AssetDatabase.Refresh();
        //}
    }

    public static void CreateBMFont(float uvX , float uvY ,Font targetFont, TextAsset fntData , Material fontMaterial , Texture2D fontTexture)
    {
        BMFont bmFont = new BMFont();
        BMFontReader.Load(bmFont, fntData.name, fntData.bytes); // 借用NGUI封装的读取类
        CharacterInfo[] characterInfo = new CharacterInfo[bmFont.glyphs.Count];
        for (int i = 0; i < bmFont.glyphs.Count; i++)
        {
            BMGlyph bmInfo = bmFont.glyphs[i];
            CharacterInfo info = new CharacterInfo();
            info.index = bmInfo.index;
            //info.uv.x = (float)bmInfo.x / (float)bmFont.texWidth;
            //info.uv.y = 1 - (float)bmInfo.y / (float)bmFont.texHeight;
            //info.uv.width = (float)(bmInfo.width) / (float)bmFont.texWidth;
            // info.uv.height = -1f * (float)(bmInfo.height) / (float)bmFont.texHeight;

            info.uv.x = (float)(bmInfo.x + uvX) / (float)fontTexture.width;
            info.uv.y = 1 - (float)(bmInfo.y + uvY) / (float)fontTexture.height;
            info.uv.width = (float)(bmInfo.width) / (float)fontTexture.width;
            info.uv.height = -1f * (float)(bmInfo.height) / (float)fontTexture.height;

            info.vert.x = 0;
            info.vert.y = -(float)bmInfo.height;
            info.vert.width = (float)bmInfo.width;
            info.vert.height = (float)bmInfo.height;
            info.width = (float)bmInfo.advance;
            characterInfo[i] = info;
        }
        targetFont.characterInfo = characterInfo;
        if (fontMaterial)
        {
            fontMaterial.mainTexture = fontTexture;
        }
        targetFont.material = fontMaterial;
        //fontMaterial.shader = Shader.Find("UI/Default");//这一行很关键，如果用standard的shader，放到Android手机上，第一次加载会很慢

        Debug.Log("create font <" + targetFont.name + "> success");
        //Close();

        EditorUtility.SetDirty(targetFont);
        EditorUtility.SetDirty(fontMaterial);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

    }
}
