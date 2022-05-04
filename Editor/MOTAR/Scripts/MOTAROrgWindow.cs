using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Unity;
using Unity.EditorCoroutines.Editor;
using System.Collections;
using System.Text;
using UnityEngine.SceneManagement;

using System.Collections.Generic;

using UnityEngine.Networking;

using DXCommunications;



public class MOTAROrgWindow : EditorWindow
{

    private static DXConfiguration dxConfiguration;
    public static bool WasActive = false;
    public static bool IsActive = false;
    public static EditorWindow myWindow;
    public static GUIContent focusedTitle;
    public static GUIContent unfocusedTitle;
    public static GUIContent currentTitle;
    public static MOTAROrgWindow instance;
    public static VisualElement root;

    public static DateTime StartedShowingClientID;
    public static DateTime StartedShowingClientSecret;
    private static TextField clientID;
    private static TextField clientSecret;
    private static string appId;
    public static DXAppLessonData thisAppsTest;

    private string userId;
    private static List<DXDeveloperCompany> companyList;

    // [MenuItem("MOTAR Prefabs/Open _%#T")]

    private void OnEnable()
    {
        instance = this;
        if (myWindow == null)
            myWindow = GetWindow<MOTAROrgWindow>();

        //Reference to the root of the window.
        root = rootVisualElement;

        var quickToolVisualTree = Resources.Load<VisualTreeAsset>("MOTAROrgSelection");
        quickToolVisualTree.CloneTree(root);

        var image = root.Q<Image>("MOTARLOGO");
        image.image = Resources.Load<Texture2D>("MOTAR_SDK_LOGO");
        
        image.style.left = 50;

        var OrgBox = root.Query<Box>().ToList().Find(x => x.name == "ORGBOX");
        OrgBox.style.left = 50 + ((image.image.width / 2) - 165);

        myWindow.maxSize = new Vector2(612, 400);
        myWindow.minSize = myWindow.maxSize;

        var submit = root.Q<Button>("MOTARORGSELECT");

        submit.clicked += SubmitOrgSelection;

        PopulateOrgList();
    }

    private void PopulateOrgList()
    {
        if (companyList != null && companyList.Count > 1)
        {
            List<string> companies = new List<string>();
            foreach (DXDeveloperCompany company in companyList)
                companies.Add(company.name);

            var orgBox = root.Q<Box>("ORGBOX");
            DropdownField dtf = new DropdownField(companies, 0);

            orgBox.Add((VisualElement) dtf);

            dtf.name = "ORGS";

            dtf.style.top = 0;
            dtf.style.left = 90;
            dtf.style.width = 170;
        }
    }
    private void SubmitOrgSelection()
    {
        DXDeveloperCompany dxCompany;

        var dd = root.Q<DropdownField>("ORGS");
        dxCompany = companyList.Find(x => x.name == dd.value);

        EditorCoroutineUtility.StartCoroutine(CompleteSubmission(dxCompany), this);
        myWindow.Close();
    }

    public static IEnumerator CompleteSubmission(DXDeveloperCompany dxCompany)
    {
        yield return DXCommunicationLayerEditor.MOTARDeveloperEnumerateCompanyApps(dxCompany);
        yield return DXCommunicationLayerEditor.MOTARDeveloperEnumerateCompanySandboxUsers(dxCompany);

        MOTARSdk.ShowAuthenticatedWindows();
    }

    public static void PopulateDXCompanies(List<DXDeveloperCompany> companies)
    {
        if (companies != null)
            companyList = companies;
    }

}
