public static class TasksTranslater
{
    public static string DecodeTasks(int id, float task)
    {
        string message;
        switch (id) {

            case -4:
            message = "открыть " + task + " ангар рядового";
                break;

            case -3:
                message = "Выполнить все ежедневные и все еженедельные задания за неделю";
                break;

            case -2:
                message = "Выполнить " + task + " еженедельных задания";
                break;

            case -1:
                message = "Выполнить " + task + " ежедневных задания";
                break;

            case 0:
                message = "Пролететь " + FormatNumsHelper.FormatNum(task) + " м";
                break;

            case 1:
                message = "Получить " + FormatNumsHelper.FormatNum(task) + " очков";
                break;

            case 2:
                message = "Получить " + FormatNumsHelper.FormatNum(task) + " монет";
                break;

            case 3:
                message = "Пролететь " + FormatNumsHelper.FormatNum(task) + " м на скорости больше 500 км/ч";
                break;

            case 4:
                message = "Получить " + FormatNumsHelper.FormatNum(task) + " очков на скорости больше 500 км/ч";
                break;

            case 5:
                message = "Открыть " + task + " ангаров";
                break;

            case 6:
                message = "Провести в небе " + FormatNumsHelper.FormatTime(task);
                break;

            case 7:
                message = "Пролететь " + FormatNumsHelper.FormatNum(task) + " м на скорости меньше 500 км/ч";
                break;

            case 8:
                message = "Получить " + FormatNumsHelper.FormatNum(task) + " очков на скорости меньше 500 км/ч";
                break;

            case 9:
                message = "Устроить " + task + " вылетов";
                break;

            case 10:
                message = "Открыть ангаров на " + FormatNumsHelper.FormatNum(task) + " монет";
                break;

            case 11:
                message = "Получить " + FormatNumsHelper.FormatNum(task) + " монет из ангаров";
                break;

            case 12:
                message = "Получить " + FormatNumsHelper.FormatNum(task) + " монет с вылетов";
                break;

            default:
                message = "error";
                break;
        }
        return message;
    }

    public static string DecodeTrials(int id, float task, string planeName)
    {
        string message = id switch
        {
            0 => "Пролететь " + FormatNumsHelper.FormatNum(task) + " м на " + planeName,
            1 => "Получить " + FormatNumsHelper.FormatNum(task) + " очков на " + planeName,
            2 => "Получить " + FormatNumsHelper.FormatNum(task) + " монет на " + planeName,
            3 => "Провести в небе " + FormatNumsHelper.FormatTime(task) + " на " + planeName,
            4 => "Устроить " + task + " вылетов" + " на " + planeName,
            _ => "error",
        };
        return message;
    }
}