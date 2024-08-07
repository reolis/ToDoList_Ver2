using Android.Content;
using Android.Health.Connect.DataTypes.Units;
using Android.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList_Ver2
{
    internal class TaskAdapter : BaseAdapter<Task>
    {
        private List<Task> items;
        private Context context;

        public TaskAdapter(Context context, List<Task> items)
        {
            this.items = items;
            this.context = context;
        }

        public override Task this[int position] => items[position];

        public override int Count => items.Count;

        public override long GetItemId(int position) => position;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? LayoutInflater.From(context).Inflate(Android.Resource.Layout.SimpleListItem1, null);
            var task = items[position];
            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = task.Title;
            return view;
        }
    }
}
