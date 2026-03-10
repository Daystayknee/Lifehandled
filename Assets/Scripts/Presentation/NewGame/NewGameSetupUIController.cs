using Lifehandled.Application.UseCases.NewGame;
using Lifehandled.Domain.Common;
using Lifehandled.Infrastructure.Genetics;
using Lifehandled.Infrastructure.Persistence.Stores;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Lifehandled.Presentation.NewGame
{
    /// <summary>
    /// Minimal V1 New Game flow controller:
    /// - main character input
    /// - optional household member panel
    /// - confirm/start with basic validation
    /// - transition to first playable scene
    /// </summary>
    public class NewGameSetupUIController : MonoBehaviour
    {
        [Header("Character Setup")]
        [SerializeField] private InputField mainCharacterNameInput;

        [Header("Optional Household Member")]
        [SerializeField] private Toggle includeMemberToggle;
        [SerializeField] private GameObject optionalMemberPanel;
        [SerializeField] private InputField optionalMemberNameInput;
        [SerializeField] private Dropdown relationshipTypeDropdown;

        [Header("Actions")]
        [SerializeField] private Button startButton;
        [SerializeField] private Text validationMessageText;

        [Header("Scene Transition")]
        [SerializeField] private string firstPlayableSceneName = "MainScene";

        private InitializeNewGameUseCase _initializeNewGameUseCase;

        private void Awake()
        {
            var saveStore = new JsonNewGameSaveStore();
            var founderGeneticsFactory = new FounderGeneticsFactory();
            _initializeNewGameUseCase = new InitializeNewGameUseCase(saveStore, founderGeneticsFactory);

            if (includeMemberToggle != null)
            {
                includeMemberToggle.onValueChanged.AddListener(OnIncludeMemberChanged);
                OnIncludeMemberChanged(includeMemberToggle.isOn);
            }

            if (startButton != null)
            {
                startButton.onClick.AddListener(OnStartClicked);
            }

            if (validationMessageText != null)
            {
                validationMessageText.text = string.Empty;
            }
        }

        private void OnDestroy()
        {
            if (includeMemberToggle != null)
            {
                includeMemberToggle.onValueChanged.RemoveListener(OnIncludeMemberChanged);
            }

            if (startButton != null)
            {
                startButton.onClick.RemoveListener(OnStartClicked);
            }
        }

        private void OnIncludeMemberChanged(bool includeMember)
        {
            if (optionalMemberPanel != null)
            {
                optionalMemberPanel.SetActive(includeMember);
            }
        }

        private void OnStartClicked()
        {
            if (!ValidateInputs(out var error))
            {
                SetValidationMessage(error);
                return;
            }

            var request = new NewGameSetupRequest
            {
                mainCharacterName = mainCharacterNameInput != null ? mainCharacterNameInput.text.Trim() : "Player",
                includeHouseholdMember = includeMemberToggle != null && includeMemberToggle.isOn,
                householdMemberName = optionalMemberNameInput != null ? optionalMemberNameInput.text.Trim() : "Housemate",
                optionalMemberRelationshipType = ResolveRelationshipType()
            };

            _initializeNewGameUseCase.Execute(request);
            SceneManager.LoadScene(firstPlayableSceneName);
        }

        private bool ValidateInputs(out string error)
        {
            if (mainCharacterNameInput == null || string.IsNullOrWhiteSpace(mainCharacterNameInput.text))
            {
                error = "Please enter a name for your main character.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(firstPlayableSceneName))
            {
                error = "First playable scene name is not configured.";
                return false;
            }

            var includeMember = includeMemberToggle != null && includeMemberToggle.isOn;
            if (includeMember && (optionalMemberNameInput == null || string.IsNullOrWhiteSpace(optionalMemberNameInput.text)))
            {
                error = "Please enter a name for the optional household member.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        private RelationshipType ResolveRelationshipType()
        {
            if (relationshipTypeDropdown == null)
            {
                return RelationshipType.Roommate;
            }

            if (relationshipTypeDropdown.value == 1)
            {
                return RelationshipType.Friend;
            }

            if (relationshipTypeDropdown.value == 2)
            {
                return RelationshipType.Family;
            }

            return RelationshipType.Roommate;
        }

        private void SetValidationMessage(string message)
        {
            if (validationMessageText != null)
            {
                validationMessageText.text = message;
            }
        }
    }
}
