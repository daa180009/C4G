using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;
using TMPro;

public class DeckBuildingController : MonoBehaviour
{
	public GameObject collectionPrefab;
	public GameObject collectionGrid;
	public Dropdown deckListDropdown;
	public Dropdown stageListDropdown;
	public InputField deckListName;
	public TextMeshProUGUI deckListText;
	public TextMeshProUGUI deckSizeRequirement;
	public WorldInfo worldInfo;

	public int MinDeckSize = 20;

	DeckListData data = new DeckListData();
	CardData[] allCards;
	StageData[] allStages;
	List<string> savedDeckNames;
	bool editingName = false;
	int deckSize = 0;

	string deckTitle = "Decklist";
	StageData selectedStage;

	string GetFileDirectory()
	{
		return Application.persistentDataPath + "/SavedDecks";
	}

	string GetFilename(string title)
	{
		return GetFileDirectory() + "/" + title + ".json";
	}

	void updateDeckNameList() {
		savedDeckNames = Directory.GetFiles(GetFileDirectory(), "*.json").Select(i => i.Split('\\').Last().Split('.').First()).ToList();
	}

	void Start()
	{
		// initialize card list
		allCards = Resources.LoadAll("Cards", typeof(CardData)).Cast<CardData>().Where(n => n.canBuildWith).ToArray();

		// initialize stage dropdown
		allStages = Resources.LoadAll("Stages", typeof(StageData)).Cast<StageData>().ToArray();
		List<string> stageListDropdownOptions = new List<string>();
		foreach (StageData stage in allStages)
			stageListDropdownOptions.Add(stage.StageTitle);
		selectedStage = allStages[0];
		stageListDropdown.ClearOptions();
		stageListDropdown.AddOptions(stageListDropdownOptions);

		// create decklist directory
		Directory.CreateDirectory(GetFileDirectory());
		updateDeckNameList();

		if (savedDeckNames.Count > 0)
			deckTitle = savedDeckNames[0];

		populateDeckCollection();
		loadDeck();
	}

	void populateDeckCollection()
	{
		foreach (CardData card in allCards)
		{
			GameObject collectionObject = Instantiate(collectionPrefab, new Vector3(0, 0, 0), Quaternion.identity);
			CollectionCardController collectionController = collectionObject.GetComponent<CollectionCardController>();

			collectionObject.transform.SetParent(collectionGrid.transform);
			collectionController.Data = card;
			collectionController.RegisterCountChanged((card) => updateDeckList());
		}
	}

	void updateDeckList()
	{
		// update list of decks
		updateDeckNameList();
		List<string> deckListDropdownOptions = new List<string>(savedDeckNames);
		deckListDropdownOptions.Add("Create new...");

		deckListDropdown.ClearOptions();
		deckListDropdown.AddOptions(deckListDropdownOptions);

		int selectedDeckIndex = savedDeckNames.IndexOf(deckTitle);
		if (selectedDeckIndex != -1 && selectedDeckIndex < savedDeckNames.Count)
		{
			deckListDropdown.value = selectedDeckIndex;
		}

		// update current deck
		deckListText.text = "";
		deckSize = 0;
		List<CardData> cardsList = new List<CardData>();

		foreach(Transform child in collectionGrid.transform)
        {
			CollectionCardController collectionCardController = child.GetComponent<CollectionCardController>();

			if(collectionCardController != null && collectionCardController.Count != 0)
            {
				int maxCount = collectionCardController.MaxCount;

				string cardDescription = collectionCardController.Count + (maxCount != -1 ? "/" + (maxCount) : "") + " " + collectionCardController.Data.CardTitle + "\n";
				deckListText.text += cardDescription;
				deckSize += collectionCardController.Count;

				for (int i = 0; i < collectionCardController.Count; i++)
                {
					cardsList.Add(collectionCardController.Data);
                }
            }
        }

		if(deckSize < MinDeckSize)
		{
			deckSizeRequirement.text = deckSize + "/20 Required";
		}
        else
        {
			deckSizeRequirement.text = "";
		}

		data.Cards = cardsList;
	}

	void saveDeck()
	{
		string dataString = JsonUtility.ToJson(data);
		File.WriteAllText(GetFilename(deckTitle), dataString);
	}

	void loadDeck()
	{
		if (File.Exists(GetFilename(deckTitle)))
		{
			string jsonFile = File.ReadAllText(GetFilename(deckTitle));
			data = JsonUtility.FromJson<DeckListData>(jsonFile);

			foreach (Transform child in collectionGrid.transform)
			{
				CollectionCardController collectionCardController = child.GetComponent<CollectionCardController>();

				if (collectionCardController != null)
					if (data.CardsCount.ContainsKey(collectionCardController.Data))
						collectionCardController.Count = data.CardsCount[collectionCardController.Data];
					else
						collectionCardController.Count = 0;
			}
        }
        else
        {
			saveDeck();
        }

		updateDeckList();
	}

	void deleteDeck()
	{
		File.Delete(GetFilename(deckTitle));
	}

	void submitDeckName(string newName)
	{
		deleteDeck();
		deckTitle = newName;
		saveDeck();
		updateDeckList();
	}

	void newDeck()
	{
		deckTitle = "New Deck";

		int newDeckCount = 1;
		if(savedDeckNames.Contains("New Deck"))
        {
			while (savedDeckNames.Contains("New Deck " + newDeckCount))
				newDeckCount += 1;
			deckTitle = "New Deck " + newDeckCount;	
        }

		data = new DeckListData();
		saveDeck();
		loadDeck();
		updateDeckList();
    }

	public void FixedUpdate()
    {
		float cardWidth = 112f;
		float gridWidth = collectionGrid.transform.parent.GetComponent<RectTransform>().rect.width;
		if (collectionGrid.transform.childCount > 0)
			cardWidth = collectionGrid.transform.GetChild(0).GetComponent<RectTransform>().rect.width;

		collectionGrid.GetComponent<GridLayoutGroup>().constraintCount = (int)Mathf.Floor((gridWidth - 20f) / (cardWidth + 20f));
	}

	public void OnStageSelected(int _)
	{
		selectedStage = allStages[stageListDropdown.value];
    }

	public void OnDeckSelected(int _)
    {
		if(deckListDropdown.value == savedDeckNames.Count)
        {
			newDeck();
			return;
        }

		if (deckListDropdown.value > savedDeckNames.Count || deckTitle == savedDeckNames[deckListDropdown.value])
			return;

		deckTitle = savedDeckNames[deckListDropdown.value];
		loadDeck();
    }

	public void OnToggleRename()
    {
		editingName = !editingName;
        if (editingName)
		{
			// activate the text input field and bring it to the front
			deckListName.Select();
			deckListName.ActivateInputField();
			deckListDropdown.transform.SetAsFirstSibling();
			deckListName.transform.SetAsLastSibling();
			deckListName.text = deckTitle;
		}
        else
        {
			// rename the current deck and move deck selector back to the front
			submitDeckName(deckListName.text);
			deckListName.transform.SetAsFirstSibling();
			deckListDropdown.transform.SetAsLastSibling();
		}
    }

	public void OnMainMenu()
	{
		SceneManager.LoadSceneAsync("MainMenu");
	}

	public void OnGameStart()
    {
		if (deckSize < 20)
			return;

		PlayerChoices.DeckList = data;
		PlayerChoices.SelectedStage = selectedStage;

		worldInfo.Restart();
		File.Delete(Application.persistentDataPath + "/SavedGames" + "/" + "mainSave" + ".json");
		SceneManager.LoadSceneAsync("Level");
    }

    public void OnDeckSave()
	{
		saveDeck();
	}

	public void OnDeckDelete()
    {
		if(savedDeckNames.Count > 1)
		{
			deleteDeck();
			if (savedDeckNames[0] == deckTitle)
				deckTitle = savedDeckNames[1];
			else
				deckTitle = savedDeckNames[0];
			loadDeck();
		}
    }
}
