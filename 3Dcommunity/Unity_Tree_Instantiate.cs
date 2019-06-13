/*Autor-CatKucha Date-2019.06 
  This script should be used as a component of the camera of your FPS controller*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System;

public class Unity_Tree_Instantiate : MonoBehaviour
{
    private Camera _camera;
    public Transform Transform_human;
    public GameObject arborPrefab;
    public GameObject shrubPrefab;
    public GameObject vinePrefab;
    public GameObject First_date_sample;
    public GameObject Second_date_sample;
    private bool FirstIsOn = false;
    XmlDocument xmlDoc = new XmlDocument();
    void Start()
    {
        _camera = GetComponent<Camera>();
        Cursor.visible = true;

        //Read xml file
        xmlDoc.Load("Resources/blackstone-current_using.xml");//change the xml file name

        // iterate xml file
        XmlNodeList xnList = xmlDoc.SelectNodes("/trees/tree");
        foreach (XmlNode node in xnList)
        {
            float gx, gy, dbh_1, dbh_2;
            Vector3 r_rotation_1, r_rotation_2;

            string unique_ID = node["unique_ID"].InnerText;
            string growth_form = node["growth_form"].InnerText;
            string gx_string = node["gx"].InnerText;
            string gy_string = node["gy"].InnerText;
            string status_1 = node["status_1"].InnerText;
            string dbh_1_string = node["dbh_1"].InnerText;
            string r_rotation_1_string = node["r_rotation_1"].InnerText;
            string status_2 = node["status_2"].InnerText;
            string dbh_2_string = node["dbh_2"].InnerText;
            string r_rotation_2_string = node["r_rotation_2"].InnerText;

            gx = float.Parse(gx_string, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            gy = float.Parse(gy_string, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

            if ((status_1 != "No_record") && (dbh_1_string != "No_record"))
            {
                dbh_1 = float.Parse(dbh_1_string, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                r_rotation_1 = StringToVector3(r_rotation_1_string);

                GameObject tree_instantiate_1 = Instantiate(GetPrefabType(growth_form),
                    new Vector3(gx , Terrain.activeTerrain.SampleHeight(new Vector3(gx, 0, gy)), gy ),
                    Quaternion.Euler(r_rotation_1));
                tree_instantiate_1.name = unique_ID+"_1";
                tree_instantiate_1.transform.localScale = tree_instantiate_1.transform.localScale * (dbh_1/3f); //change DBH height

                int wanted_layer = First_date_sample.layer;
                Setlayer(tree_instantiate_1, wanted_layer); //change layer of root and first layer of the root
            }
            if ((status_2 != "No_record") && (dbh_2_string != "No_record"))
            {
                dbh_2 = float.Parse(dbh_2_string, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                r_rotation_2 = StringToVector3(r_rotation_2_string);

                GameObject tree_instantiate_2 = Instantiate(GetPrefabType(growth_form),
                    new Vector3(gx, Terrain.activeTerrain.SampleHeight(new Vector3(gx, 0, gy)), gy),
                    Quaternion.Euler(r_rotation_2));
                tree_instantiate_2.name = unique_ID + "_2";
                tree_instantiate_2.transform.localScale = tree_instantiate_2.transform.localScale * (dbh_2 / 3f); //change DBH height

                int wanted_layer = Second_date_sample.layer;
                Setlayer(tree_instantiate_2, wanted_layer); //change layer of root and first layer of the root
            } 

        }

        Vector3 StringToVector3(string sVector)
        {
            // Remove the parentheses
            if (sVector.StartsWith("(") && sVector.EndsWith(")"))
            {
                sVector = sVector.Substring(1, sVector.Length - 2);
            }

            // split the items
            string[] sArray = sVector.Split(',');

            // store as a Vector3
            Vector3 result = new Vector3(
                float.Parse(sArray[0]),
                float.Parse(sArray[1]),
                float.Parse(sArray[2]));

            return result;
        }

        GameObject GetPrefabType(string g_form)
        {
            if (g_form == "arbor")
            {
                return arborPrefab;
            }
            else if (g_form == "shrub")
            {
                return shrubPrefab;
            }
            else if (g_form == "Liana")
            {
                return vinePrefab;
            }
            else
            {
                return arborPrefab;
            }
        }

        void Setlayer(GameObject tree_instantiate, int wanted_layer)
        {
            tree_instantiate.layer = wanted_layer;
            foreach (Transform child in tree_instantiate.transform)
            {
                child.gameObject.layer = wanted_layer;
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (!FirstIsOn)
            {
                _camera.cullingMask = 1 | 1 << 9;
                FirstIsOn = true;
                Debug.Log("turn on");
            }
            else
            {
                _camera.cullingMask = 1 | 1 << 10;
                FirstIsOn = false;
                Debug.Log("turn off");
            }
        }

    }

    void OnGUI()
    {
        if (FirstIsOn)
        {
            GUI.Box(new Rect(0, 0, 400, 70), String.Format(
                "Press F1 to switch between first survey and second survey\r\nYour current position is gx: {0}, gy: {1}\r\nThe first survey is on",
                Transform_human.position.x.ToString("0"), Transform_human.position.z.ToString("0")));
        }
        else
        {
            GUI.Box(new Rect(0, 0, 400, 70), String.Format(
                "Press F1 to switch between first survey and second survey\r\nYour current position is gx: {0}, gy: {1}\r\nThe second survey is on",
                Transform_human.position.x.ToString("0"), Transform_human.position.z.ToString("0")));
        }

        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);    //define a ray, it shoots from your camera to your cursor
        //Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
        //Debug.Log(ray);
        RaycastHit hit;    //define a collision point
        if (FirstIsOn)
        {
            if (Physics.Raycast(ray, out hit, 10f, 1 << 9))  //if the collision happened, this ray bumps into the Gameobject which is 'hit'
            {
                //Debug.Log(hit.transform.name.GetType());
                //Debug.Log(hit.transform.name);
                string unique_ID = hit.transform.name.Replace("_1", "");
                XmlNodeList xnList = xmlDoc.SelectNodes("/trees/tree/unique_ID[text()= '" + unique_ID + "']");
                XmlNode node = xnList[0];
                XmlNode Parent_Node = node.ParentNode;
                string name_c = Parent_Node["name_c"].InnerText;
                string name_e = Parent_Node["name_e"].InnerText;
                string growth_form = Parent_Node["growth_form"].InnerText;
                string gx_string = Parent_Node["gx"].InnerText;
                string gy_string = Parent_Node["gy"].InnerText;
                string date_1 = Parent_Node["date_1"].InnerText;
                string status_1 = Parent_Node["status_1"].InnerText;
                string dbh_1_string = Parent_Node["dbh_1"].InnerText;

                string show_box = String.Format("unique_ID: {0}\r\ngx: {1}, gy: {2}\r\nname_c: {3}\r\nname_s: {4}\r\n" +
                    "growth_form: {5}\r\ndate_1: {6}\r\nstatus_1: {7}\r\ndbh_1: {8}",
                    unique_ID, gx_string, gy_string, name_c, name_e, growth_form, date_1, status_1, dbh_1_string);

                GUI.Box(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, 200, 140), show_box);
                //GUI.Box(new Rect(0, 0, 100, 100), "aaa\r\naa");
            }
        }
        else
        {
            if (Physics.Raycast(ray, out hit, 10f, 1 << 10)) 
            {
                string unique_ID = hit.transform.name.Replace("_2", "");
                XmlNodeList xnList = xmlDoc.SelectNodes("/trees/tree/unique_ID[text()= '" + unique_ID + "']");
                XmlNode node = xnList[0];
                XmlNode Parent_Node = node.ParentNode;
                string name_c = Parent_Node["name_c"].InnerText;
                string name_e = Parent_Node["name_e"].InnerText;
                string growth_form = Parent_Node["growth_form"].InnerText;
                string gx_string = Parent_Node["gx"].InnerText;
                string gy_string = Parent_Node["gy"].InnerText;
                string date_2 = Parent_Node["date_2"].InnerText;
                string status_2 = Parent_Node["status_2"].InnerText;
                string dbh_2_string = Parent_Node["dbh_2"].InnerText;

                string show_box = String.Format("unique_ID: {0}\r\ngx: {1}, gy: {2}\r\nname_c: {3}\r\nname_s: {4}\r\n" +
                    "growth_form: {5}\r\ndate_2: {6}\r\nstatus_2: {7}\r\ndbh_2: {8}",
                    unique_ID, gx_string, gy_string, name_c, name_e, growth_form, date_2, status_2, dbh_2_string);

                GUI.Box(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, 200, 140), show_box);
            }
        }
    }
}
