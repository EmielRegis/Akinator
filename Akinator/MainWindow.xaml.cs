using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Akinator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AkinatorDatabaseEntities dbContextQuestionCathegory;
        private AkinatorDatabaseEntities dbContextQuestion;
        private AkinatorDatabaseEntities dbContextCharacter;
        private AkinatorDatabaseEntities dbContextQuestionCorelation;
        private AkinatorDatabaseEntities dbContextQuestionCharacterConnection;
        private CollectionViewSource _questionCathegoryViewSource;
        private CollectionViewSource _questionCathegoriesList;
        private CollectionViewSource _questionViewSource;
        private CollectionViewSource _questionCathegoriesListRemote;
        private CollectionViewSource _characterViewSource;
        private CollectionViewSource _questionCorelationViewSource;
        private CollectionViewSource _questionsListRemote;
        private CollectionViewSource _conCharactersListRemote;
        private CollectionViewSource _conQuestionsListRemote;
        private CollectionViewSource _questionCharacterConnectionViewSource;

        private bool questionCathegoryFlag = false;
        private bool questionFlag = false;
        private bool characterFlag = false;
        private bool questionCorelationFlag = false;
        private bool questionCharacterConnectionFlag = false;

        public MainWindow()
        {
            InitializeComponent();

            dbContextQuestionCathegory = new AkinatorDatabaseEntities();
            dbContextCharacter = new AkinatorDatabaseEntities();
            dbContextQuestion = new AkinatorDatabaseEntities();
            dbContextQuestionCorelation = new AkinatorDatabaseEntities();
            dbContextQuestionCharacterConnection = new AkinatorDatabaseEntities();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dbContextQuestionCathegory.QuestionCathegories.Load();
            dbContextQuestion.Questions.Load();
            dbContextQuestion.QuestionCathegories.Load();
            dbContextCharacter.Characters.Load();
            dbContextQuestionCorelation.QuestionCorelations.Load();
            dbContextQuestionCorelation.Questions.Load();
            dbContextQuestionCharacterConnection.QuestionCharacterConnections.Load();
            dbContextQuestionCharacterConnection.Questions.Load();
            dbContextQuestionCharacterConnection.Characters.Load();


            _questionCathegoryViewSource = ((CollectionViewSource)(FindResource("questionCathegoryViewSource")));
            _questionCathegoryViewSource.Source = dbContextQuestionCathegory.QuestionCathegories.Local;

            _questionCathegoriesList = ((CollectionViewSource)(FindResource("questionCathegoriesList")));
            _questionCathegoriesList.Source = dbContextQuestionCathegory.QuestionCathegories.Local;

            _questionCathegoriesListRemote = ((CollectionViewSource)(FindResource("questionCathegoriesListRemote")));
            _questionCathegoriesListRemote.Source = dbContextQuestion.QuestionCathegories.Local;

            _questionViewSource = ((CollectionViewSource)(FindResource("questionViewSource")));
            _questionViewSource.Source = dbContextQuestion.Questions.Local;

            _characterViewSource = ((CollectionViewSource)(FindResource("characterViewSource")));
            _characterViewSource.Source = dbContextCharacter.Characters.Local;

            _questionCorelationViewSource = ((CollectionViewSource)(FindResource("questionCorelationViewSource")));
            _questionCorelationViewSource.Source = dbContextQuestionCorelation.QuestionCorelations.Local;

            _questionsListRemote = ((CollectionViewSource)(FindResource("questionsListRemote")));
            _questionsListRemote.Source = dbContextQuestionCorelation.Questions.Local;

            _questionCharacterConnectionViewSource =
                ((CollectionViewSource)(FindResource("questionCharacterConnectionViewSource")));
            _questionCharacterConnectionViewSource.Source =
                dbContextQuestionCharacterConnection.QuestionCharacterConnections.Local;

            _conCharactersListRemote = ((CollectionViewSource)(FindResource("conCharactersListRemote")));
            _conCharactersListRemote.Source = dbContextQuestionCharacterConnection.Characters.Local;

            _conQuestionsListRemote = ((CollectionViewSource)(FindResource("conQuestionsListRemote")));
            _conQuestionsListRemote.Source = dbContextQuestionCharacterConnection.Questions.Local;
        }

        private void QuestionCathegoryRowUpdate(object sender,
            DataGridRowEditEndingEventArgs dataGridRowEditEndingEventArgs)
        {
            if (QuestionCathegoriesDataGrid.SelectedItem != null)
            {
                QuestionCathegoriesDataGrid.RowEditEnding -= QuestionCathegoryRowUpdate;
                QuestionCathegoriesDataGrid.CommitEdit();
                QuestionCathegoriesDataGrid.RowEditEnding += QuestionCathegoryRowUpdate;
            }
            else
            {
                return;
            }

            var result = new QuestionCathegoryValidation().Validate(dataGridRowEditEndingEventArgs.Row.BindingGroup,
                CultureInfo.CurrentCulture);

            if (!result.IsValid)
            {
                QuestionCathegoriesSearchBox.IsEnabled = false;
                return;
            }

            QuestionCathegoriesSearchBox.IsEnabled = true;

            dbContextQuestionCathegory.SaveChanges();

            dbContextQuestion = new AkinatorDatabaseEntities();
            dbContextQuestion.QuestionCathegories.Load();
            dbContextQuestion.Questions.Load();
            _questionCathegoriesListRemote.Source = dbContextQuestion.QuestionCathegories.Local;
            _questionViewSource.Source = dbContextQuestion.Questions.Local;
            QuestionDataGrid.Items.Refresh();

            QuestionCathegoriesDataGrid.Items.Refresh();


            questionCathegoryFlag = false;
        }



        private void QuestionCathegoryViewSourceFilter(object sender, FilterEventArgs e)
        {
            QuestionCathegory cat = e.Item as QuestionCathegory;
            if (cat != null)
            {
                e.Accepted = (cat.Name.ToUpper().Contains(QuestionCathegoriesSearchBox.Text.ToUpper()));
            }
        }

        private void QuestionCathegoriesSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _questionCathegoryViewSource.Filter -= QuestionCathegoryViewSourceFilter;
            _questionCathegoryViewSource.Filter += QuestionCathegoryViewSourceFilter;
        }



        private void QuestionRowUpdate(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (QuestionDataGrid.SelectedItem != null)
            {
                QuestionDataGrid.RowEditEnding -= QuestionRowUpdate;
                QuestionDataGrid.CommitEdit();
                QuestionDataGrid.RowEditEnding += QuestionRowUpdate;
            }
            else
            {
                return;
            }

            var result = new QuestionValidation().Validate(e.Row.BindingGroup, CultureInfo.CurrentCulture);

            if (!result.IsValid)
            {
                QuestionsSearchBox.IsEnabled = false;
                return;
            }

            QuestionsSearchBox.IsEnabled = true;

            dbContextQuestion.SaveChanges();

            dbContextQuestionCorelation = new AkinatorDatabaseEntities();
            dbContextQuestionCorelation.Questions.Load();
            dbContextQuestionCorelation.QuestionCorelations.Load();
            _questionsListRemote.Source = dbContextQuestionCorelation.Questions.Local;
            _questionCorelationViewSource.Source = dbContextQuestionCorelation.QuestionCorelations.Local;
            QuestionCorelationDataGrid.Items.Refresh();

            dbContextQuestionCharacterConnection = new AkinatorDatabaseEntities();
            dbContextQuestionCharacterConnection.Questions.Load();
            dbContextQuestionCharacterConnection.QuestionCharacterConnections.Load();
            _conQuestionsListRemote.Source = dbContextQuestionCharacterConnection.Questions.Local;
            _questionCharacterConnectionViewSource.Source =
                dbContextQuestionCharacterConnection.QuestionCharacterConnections.Local;
            QuestionCharacterConnectionDataGrid.Items.Refresh();

            QuestionDataGrid.Items.Refresh();

            questionFlag = false;

        }

        private void QuestionViewSourceFilter(object sender, FilterEventArgs e)
        {
            Question cat = e.Item as Question;
            if (cat != null)
            {
                e.Accepted = (cat.Name.ToUpper().Contains(QuestionsSearchBox.Text.ToUpper()));
            }
        }

        private void QuestionsSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _questionViewSource.Filter -= QuestionViewSourceFilter;
            _questionViewSource.Filter += QuestionViewSourceFilter;
        }

        private void CharacterRowUpdate(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (CharacterDataGrid.SelectedItem != null)
            {
                CharacterDataGrid.RowEditEnding -= CharacterRowUpdate;
                CharacterDataGrid.CommitEdit();
                CharacterDataGrid.RowEditEnding += CharacterRowUpdate;
            }
            else
            {
                return;
            }

            var result = new CharacterValidation().Validate(e.Row.BindingGroup, CultureInfo.CurrentCulture);

            if (!result.IsValid)
            {
                CharactersSearchBox.IsEnabled = false;
                return;
            }

            CharactersSearchBox.IsEnabled = true;

            dbContextCharacter.SaveChanges();

            dbContextQuestionCharacterConnection = new AkinatorDatabaseEntities();
            dbContextQuestionCharacterConnection.Characters.Load();
            dbContextQuestionCharacterConnection.QuestionCharacterConnections.Load();
            _conCharactersListRemote.Source = dbContextQuestionCharacterConnection.Characters.Local;
            _questionCharacterConnectionViewSource.Source =
                dbContextQuestionCharacterConnection.QuestionCharacterConnections.Local;
            QuestionCharacterConnectionDataGrid.Items.Refresh();

            CharacterDataGrid.Items.Refresh();

            characterFlag = false;

        }

        private void CharacterViewSourceFilter(object sender, FilterEventArgs e)
        {
            Character cat = e.Item as Character;
            if (cat != null)
            {
                e.Accepted = (cat.Name.ToUpper().Contains(CharactersSearchBox.Text.ToUpper()));
            }
        }

        private void CharactersSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _characterViewSource.Filter -= CharacterViewSourceFilter;
            _characterViewSource.Filter += CharacterViewSourceFilter;
        }

        private void QuestionCorelationRowUpdate(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (QuestionCorelationDataGrid.SelectedItem != null)
            {
                QuestionCorelationDataGrid.RowEditEnding -= QuestionCorelationRowUpdate;
                QuestionCorelationDataGrid.CommitEdit();
                QuestionCorelationDataGrid.RowEditEnding += QuestionCorelationRowUpdate;
            }
            else
            {
                return;
            }

            var result = new QuestionCorelationValidation().Validate(e.Row.BindingGroup, CultureInfo.CurrentCulture);

            if (!result.IsValid)
            {
                QuestionCorelationsSearchBox.IsEnabled = false;
                return;
            }

            QuestionCorelationsSearchBox.IsEnabled = true;

            dbContextQuestionCorelation.SaveChanges();



            QuestionCorelationDataGrid.Items.Refresh();

            questionCorelationFlag = false;

        }

        private void QuestionCorelationViewSourceFilter(object sender, FilterEventArgs e)
        {
            QuestionCorelation cat = e.Item as QuestionCorelation;
            if (cat != null)
            {
                e.Accepted =
                    (dbContextQuestionCorelation.Questions.Local.First(s => s.Id == cat.QuestionA)
                        .Name.ToUpper().Contains(QuestionCorelationsSearchBox.Text.ToUpper())) ||
                    (dbContextQuestionCorelation.Questions.Local.First(s => s.Id == cat.QuestionB)
                        .Name.ToUpper().Contains(QuestionCorelationsSearchBox.Text.ToUpper()));
            }
        }

        private void QuestionCorelationsSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _questionCorelationViewSource.Filter -= QuestionCorelationViewSourceFilter;
            _questionCorelationViewSource.Filter += QuestionCorelationViewSourceFilter;
        }



        private void QuestionCharacterConnectionRowUpdate(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (QuestionCharacterConnectionDataGrid.SelectedItem != null)
            {
                QuestionCharacterConnectionDataGrid.RowEditEnding -= QuestionCharacterConnectionRowUpdate;
                QuestionCharacterConnectionDataGrid.CommitEdit();
                QuestionCharacterConnectionDataGrid.RowEditEnding += QuestionCharacterConnectionRowUpdate;
            }
            else
            {
                return;
            }

            var result = new QuestionCharacterConnectionValidation().Validate(e.Row.BindingGroup,
                CultureInfo.CurrentCulture);

            if (!result.IsValid)
            {
                QuestionCharacterConnectionsSearchBox.IsEnabled = false;
                return;
            }

            QuestionCharacterConnectionsSearchBox.IsEnabled = true;

            dbContextQuestionCharacterConnection.SaveChanges();

            QuestionCharacterConnectionDataGrid.Items.Refresh();

            questionCharacterConnectionFlag = false;

        }

        private void QuestionCharacterConnectionViewSourceFilter(object sender, FilterEventArgs e)
        {
            QuestionCharacterConnection cat = e.Item as QuestionCharacterConnection;
            if (cat != null)
            {
                dbContextQuestionCharacterConnection.Questions.Load();
                dbContextQuestionCharacterConnection.Characters.Load();


                e.Accepted =
                    (dbContextQuestionCharacterConnection.Characters.Local.First(d => d.Id == cat.Character)
                        .Name.ToUpper().Contains(QuestionCharacterConnectionsSearchBox.Text.ToUpper())) ||
                    (dbContextQuestionCharacterConnection.Questions.Local.First(s => s.Id == cat.Question)
                        .Name.ToUpper().Contains(QuestionCharacterConnectionsSearchBox.Text.ToUpper()));
            }
        }

        private void QuestionCharacterConnectionsSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _questionCharacterConnectionViewSource.Filter -= QuestionCharacterConnectionViewSourceFilter;
            _questionCharacterConnectionViewSource.Filter += QuestionCharacterConnectionViewSourceFilter;
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((!(e.Source is TabControl)) || e.RemovedItems.Count == 0 || e.AddedItems.Count == 0)
            {
                //                QuestionDataGrid.
                return;
            }

            if (QuestionsTab.IsSelected)
            {
                if (!questionCathegoryFlag)
                {
                    dbContextQuestionCathegory.SaveChanges();

                    dbContextQuestion = new AkinatorDatabaseEntities();
                    dbContextQuestion.QuestionCathegories.Load();
                    dbContextQuestion.Questions.Load();
                    _questionCathegoriesListRemote.Source = dbContextQuestion.QuestionCathegories.Local;
                    _questionViewSource.Source = dbContextQuestion.Questions.Local;
                    QuestionDataGrid.Items.Refresh();
                }
            }
            else if (QuestionCorelationsTab.IsSelected)
            {
                if (!characterFlag)
                {
                    dbContextCharacter.SaveChanges();

                    dbContextQuestionCorelation = new AkinatorDatabaseEntities();
                    dbContextQuestionCorelation.Questions.Load();
                    dbContextQuestionCorelation.QuestionCorelations.Load();
                    _questionsListRemote.Source = dbContextQuestionCorelation.Questions.Local;
                    _questionCorelationViewSource.Source = dbContextQuestionCorelation.QuestionCorelations.Local;
                    QuestionCorelationDataGrid.Items.Refresh();
                }
            }
            else if (QuestionCharacterConnectionTab.IsSelected)
            {
                if (!questionFlag)
                {
                    dbContextQuestion.SaveChanges();

                    dbContextQuestionCharacterConnection = new AkinatorDatabaseEntities();
                    dbContextQuestionCharacterConnection.Questions.Load();
                    dbContextQuestionCharacterConnection.QuestionCharacterConnections.Load();
                    _conQuestionsListRemote.Source = dbContextQuestionCharacterConnection.Questions.Local;
                    _questionCharacterConnectionViewSource.Source =
                        dbContextQuestionCharacterConnection.QuestionCharacterConnections.Local;
                    QuestionCharacterConnectionDataGrid.Items.Refresh();
                }

                if (!characterFlag)
                {
                    dbContextCharacter.SaveChanges();

                    dbContextQuestionCharacterConnection = new AkinatorDatabaseEntities();
                    dbContextQuestionCharacterConnection.Characters.Load();
                    dbContextQuestionCharacterConnection.QuestionCharacterConnections.Load();
                    _conCharactersListRemote.Source = dbContextQuestionCharacterConnection.Characters.Local;
                    _questionCharacterConnectionViewSource.Source =
                        dbContextQuestionCharacterConnection.QuestionCharacterConnections.Local;
                    QuestionCharacterConnectionDataGrid.Items.Refresh();
                }
            }
        }

        private void QuestionDataGrid_OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            questionFlag = true;
        }

        private void QuestionCorelationDataGrid_OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            questionCorelationFlag = true;
        }

        private void QuestionCharacterConnectionDataGrid_OnBeginningEdit(object sender,
            DataGridBeginningEditEventArgs e)
        {
            questionCharacterConnectionFlag = true;
        }

        private void QuestionCathegoriesDataGrid_OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            questionCathegoryFlag = true;

        }

        private void CharacterDataGrid_OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            characterFlag = true;
        }


    }

    public class QuestionCathegoryValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            QuestionCathegory cathegory = (value as BindingGroup).Items[0] as QuestionCathegory;
            if (cathegory.Name == null || cathegory.Name.Length == 0)
            {
                return new ValidationResult(false, "The name value can't be empty");
            }
            if (cathegory.Name != null && cathegory.Name.Length > 50)
            {
                return new ValidationResult(false, "Cathegory name is too long. Length should be smaller than 50 characters");
            }
            if (cathegory.ParentCathegory != null && cathegory.ParentCathegory.ToString() == cathegory.Id.ToString())
            {
                return new ValidationResult(false, "Cathegory can not be 'parent cathegory' for its self");
            }

            AkinatorDatabaseEntities tempContext = new AkinatorDatabaseEntities();

            if (tempContext.QuestionCathegories.Count(s => (s.Name == cathegory.Name) && (s.Id != cathegory.Id)) > 0)
            {
                return new ValidationResult(false, "Cathegory names have to be unique");
            }

            return ValidationResult.ValidResult;
        }
    }

    public class QuestionValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Question question = (value as BindingGroup).Items[0] as Question;
            if (question.Name == null || question.Name.Length == 0)
            {
                return new ValidationResult(false, "The name value can't be empty");
            }
            if (question.Name != null && question.Name.Length > 50)
            {
                return new ValidationResult(false, "Question name is too long. Length should be smaller than 50 characters");
            }
            if (question.GeneralizationDegree < 0 || question.GeneralizationDegree > 100)
            {
                return new ValidationResult(false, "Value of generalization should be in range (0, 100)");
            }

            AkinatorDatabaseEntities tempContext = new AkinatorDatabaseEntities();

            if (tempContext.Questions.Count(s => (s.Name == question.Name) && (s.Id != question.Id)) > 0)
            {
                return new ValidationResult(false, "Question names have to be unique");
            }

            return ValidationResult.ValidResult;
        }
    }

    public class CharacterValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Character character = (value as BindingGroup).Items[0] as Character;
            if (character.Name == null || character.Name.Length == 0)
            {
                return new ValidationResult(false, "The name value can't be empty");
            }
            if (character.Name != null && character.Name.Length > 50)
            {
                return new ValidationResult(false, "Character name is too long. Length should be smaller than 50 characters");
            }
            if (character.Description != null && character.Description.Length > 50)
            {
                return new ValidationResult(false, "Character description is too long. Length should be smaller than 50 characters");
            }

            if (character.OccurencesNumber < 0)
            {
                return new ValidationResult(false, "Value of occurence number shall be greater or equal 0");
            }

            AkinatorDatabaseEntities tempContext = new AkinatorDatabaseEntities();

            if (tempContext.Characters.Count(s => (s.Name == character.Name) && (s.Id != character.Id)) > 0)
            {
                return new ValidationResult(false, "Character names have to be unique");
            }

            return ValidationResult.ValidResult;
        }
    }

    public class QuestionCorelationValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            QuestionCorelation qCor = (value as BindingGroup).Items[0] as QuestionCorelation;


            if (qCor.QuestionA == 0)
            {
                return new ValidationResult(false, "You have to choose one question A option");
            }

            if (qCor.QuestionB == 0)
            {
                return new ValidationResult(false, "You have to choose one question B option");
            }

            if (qCor.CommonCases < 0)
            {
                return new ValidationResult(false, "Common case value shall be greater or equal 0");
            }

            if ((qCor.CorelationPower < 0) || (qCor.CorelationPower > 100))
            {
                return new ValidationResult(false, "Corelation power value shall be in range (0, 100)");
            }

            if ((qCor.CorelationDirection < -100) || (qCor.CorelationDirection > 100))
            {
                return new ValidationResult(false, "Corelation direction power value shall be in range (-100, 100)");
            }

            if (qCor.QuestionA == qCor.QuestionB)
            {
                return new ValidationResult(false, "Questions A and B cannot be the same");
            }

            AkinatorDatabaseEntities tempContext = new AkinatorDatabaseEntities();

            if (tempContext.QuestionCorelations
                .Count(s => (((s.QuestionA == qCor.QuestionA) && (s.QuestionB == qCor.QuestionB) && (s.Id != qCor.Id)) ||
                             ((s.QuestionA == qCor.QuestionB) && (s.QuestionB == qCor.QuestionA) && (s.Id != qCor.Id)))) > 0)
            {
                return new ValidationResult(false, "Question corelations have to be unique");
            }

            return ValidationResult.ValidResult;
        }
    }

    public class QuestionCharacterConnectionValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            QuestionCharacterConnection cQCCon = (value as BindingGroup).Items[0] as QuestionCharacterConnection;

            if (cQCCon.Character == 0)
            {
                return new ValidationResult(false, "You have to choose one character option");
            }

            if (cQCCon.Question == 0)
            {
                return new ValidationResult(false, "You have to choose one question option");
            }


            if (cQCCon.OccurencesNumber < 0)
            {
                return new ValidationResult(false, "Occurence number value shall be greater or equal 0");
            }

            if (cQCCon.YesAnswers < 0)
            {
                return new ValidationResult(false, "Yes answers value shall be greater or equal 0");
            }

            if (cQCCon.ProbablyYesAnswers < 0)
            {
                return new ValidationResult(false, "Probably yes value shall be greater or equal 0");
            }

            if (cQCCon.DontKnowAnswers < 0)
            {
                return new ValidationResult(false, "Don't know value shall be greater or equal 0");
            }

            if (cQCCon.ProbablyNotAnswers < 0)
            {
                return new ValidationResult(false, "Probably not value shall be greater or equal 0");
            }

            if (cQCCon.NotAnswers < 0)
            {
                return new ValidationResult(false, "Not answers value shall be greater or equal 0");
            }

            if ((cQCCon.ProbabilisticEvaluation < 0.0) || (cQCCon.ProbabilisticEvaluation > 1.0))
            {
                return new ValidationResult(false, "Probabilisic evaluation value shall be in range (0.0, 1.0)");
            }

            AkinatorDatabaseEntities tempContext = new AkinatorDatabaseEntities();

            if (tempContext.QuestionCharacterConnections
                .Count(s => ((s.Character == cQCCon.Character) && (s.Id != cQCCon.Id) && (s.Question == cQCCon.Question))) > 0)
            {
                return new ValidationResult(false, "Character Question connection have to be unique");
            }


            return ValidationResult.ValidResult;
        }
    }
}
