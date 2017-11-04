using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawerMgr {

    private DrawerMgr() { }

    public static Material GetDrawerMaterial()
    {
        Material mat = Resources.Load<Material>("Shader/Colored Blended");
        mat.hideFlags = HideFlags.HideAndDontSave;
        mat.shader.hideFlags = HideFlags.HideAndDontSave;
        return mat;
    }
	
}
