using Android.Graphics.Drawables;
using Android.Views;
using System.Drawing;

namespace ToDoList_Ver2
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        Button btnDel;
        Button btnAdd;
        Button btnEdit;

        EditText enterText;
        TextView taskTextView;

        List<Task> tasks = new List<Task>();

        bool isBtnAddClicked = false;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            btnDel = FindViewById<Button>(Resource.Id.btnDel);
            btnAdd = FindViewById<Button>(Resource.Id.btnAdd);
            btnEdit = FindViewById<Button>(Resource.Id.btnEdit);

            enterText = FindViewById<EditText>(Resource.Id.editText1);
            taskTextView = FindViewById<TextView>(Resource.Id.task);

            // Установите обработчики событий для кнопок
            btnDel.Click += BtnDel_Click;
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
        }

        private void BtnAdd_Click(object sender, System.EventArgs e)
        {
            // Проверка текущей видимости EditText и переключение
            if (enterText.Visibility == ViewStates.Invisible)
            {
                enterText.Visibility = ViewStates.Visible; // Делаем видимым
                string taskText = enterText.Text;
                if (taskText != null)
                {
                    taskTextView.Text = taskText;
                }
            }
            else
            {
                enterText.Visibility = ViewStates.Invisible; // Делаем невидимым
            }

            

            btnAdd.SetBackgroundColor(Android.Graphics.Color.Argb(100, 202, 130, 248));
            isBtnAddClicked = true;
            Task task = new Task();
            task.Title = taskTextView.Text;

            tasks.Add(task);

            if (isBtnAddClicked)
            {
                btnAdd.SetBackgroundColor(Android.Graphics.Color.Argb(100, 242, 187, 187));
            }
            Toast.MakeText(this, "Task is added", ToastLength.Short).Show();
        }

        private void BtnEdit_Click(object sender, System.EventArgs e)
        {

        }

        private void BtnDel_Click(object sender, System.EventArgs e)
        {

        }
    }
}