using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Base : MonoBehaviour {
    private AppFacade m_Facade;
    private ResourceManager m_ResMgr;


    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    protected AppFacade facade {
        get {
            if (m_Facade == null) {
                m_Facade = AppFacade.Instance;
            }
            return m_Facade;
        }
    }
    protected ResourceManager ResManager {
        get {
            if (m_ResMgr == null)
            {
                m_ResMgr = this.GetComponent<ResourceManager>();
            }
            return m_ResMgr;
        }
    }

}
