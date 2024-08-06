using Android.App;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using System.Collections.Generic;
using System.Drawing;

namespace ToDoList_Ver2
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        Button btnDel;
        Button btnAdd;
        Button btnEdit;
        Button btnCreate;

        EditText enterText;
        TextView taskTextView;

        List<string> tasks = new List<string>();

        bool isBtnAddClicked = false;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_main);

            btnDel = FindViewById<Button>(Resource.Id.btnDel);
            btnAdd = FindViewById<Button>(Resource.Id.btnAdd);
            btnEdit = FindViewById<Button>(Resource.Id.btnEdit);
            btnCreate = FindViewById<Button>(Resource.Id.btnCreate);

            enterText = FindViewById<EditText>(Resource.Id.editText1);
            taskTextView = FindViewById<TextView>(Resource.Id.task);

            btnDel.Click += BtnDel_Click;
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnCreate.Click += BtnCreate_Click;

            enterText.EditorAction += (object sender, TextView.EditorActionEventArgs e) =>
            {
                if (e.ActionId == ImeAction.Done)
                {
                    AddTask();
                    HideKeyboard();
                    e.Handled = true;
                }
                else
                {
                    e.Handled = false;
                }
            };
        }

        private void BtnAdd_Click(object sender, System.EventArgs e)
        {
            if (enterText.Visibility == ViewStates.Invisible)
            {
                btnCreate.Visibility = ViewStates.Visible;
                enterText.Visibility = ViewStates.Visible;
                enterText.RequestFocus();
                ShowKeyboard();
            }
            else
            {
                enterText.Visibility = ViewStates.Invisible;
                btnCreate.Visibility = ViewStates.Invisible;
                HideKeyboard();
            }

            btnAdd.SetBackgroundColor(Android.Graphics.Color.Argb(100, 202, 130, 248));
            isBtnAddClicked = true;

            if (isBtnAddClicked)
            {
                btnAdd.SetBackgroundColor(Android.Graphics.Color.Argb(100, 242, 187, 187));
                isBtnAddClicked = false;
            }
        }

        private void BtnEdit_Click(object sender, System.EventArgs e)
        {
            // Реализация функционала редактирования задачи
        }

        private void BtnDel_Click(object sender, System.EventArgs e)
        {
            // Реализация функционала удаления задачи
        }

        private void BtnCreate_Click(object sender, System.EventArgs e)
        {
            AddTask();
            btnAdd.SetBackgroundColor(Android.Graphics.Color.Argb(100, 242, 187, 187));
        }

        private void AddTask()
        {
            string taskText = enterText.Text;
            if (!string.IsNullOrWhiteSpace(taskText))
            {
                tasks.Add(taskText);
                enterText.Text = string.Empty;
                enterText.Visibility = ViewStates.Invisible;
                btnCreate.Visibility = ViewStates.Invisible;
                taskTextView.Text = string.Join("\n", tasks);
                Toast.MakeText(this, "Task is added", ToastLength.Short).Show();
            }
            else
            {
                Toast.MakeText(this, "Task cannot be empty", ToastLength.Short).Show();
            }
        }

        private void ShowKeyboard()
        {
            InputMethodManager imm = (InputMethodManager)GetSystemService(InputMethodService);
            imm.ShowSoftInput(enterText, ShowFlags.Implicit);
        }

        private void HideKeyboard()
        {
            InputMethodManager imm = (InputMethodManager)GetSystemService(InputMethodService);
            imm.HideSoftInputFromWindow(enterText.WindowToken, 0);
        }
    }
}
