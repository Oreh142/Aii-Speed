public static class TasksTranslater
{
    public static string DecodeTasks(int id, float task)
    {
        string message;
        switch (id) {

            case -4:
            message = "������� " + task + " ����� ��������";
                break;

            case -3:
                message = "��������� ��� ���������� � ��� ������������ ������� �� ������";
                break;

            case -2:
                message = "��������� " + task + " ������������ �������";
                break;

            case -1:
                message = "��������� " + task + " ���������� �������";
                break;

            case 0:
                message = "��������� " + FormatNumsHelper.FormatNum(task) + " �";
                break;

            case 1:
                message = "�������� " + FormatNumsHelper.FormatNum(task) + " �����";
                break;

            case 2:
                message = "�������� " + FormatNumsHelper.FormatNum(task) + " �����";
                break;

            case 3:
                message = "��������� " + FormatNumsHelper.FormatNum(task) + " � �� �������� ������ 500 ��/�";
                break;

            case 4:
                message = "�������� " + FormatNumsHelper.FormatNum(task) + " ����� �� �������� ������ 500 ��/�";
                break;

            case 5:
                message = "������� " + task + " �������";
                break;

            case 6:
                message = "�������� � ���� " + FormatNumsHelper.FormatTime(task);
                break;

            case 7:
                message = "��������� " + FormatNumsHelper.FormatNum(task) + " � �� �������� ������ 500 ��/�";
                break;

            case 8:
                message = "�������� " + FormatNumsHelper.FormatNum(task) + " ����� �� �������� ������ 500 ��/�";
                break;

            case 9:
                message = "�������� " + task + " �������";
                break;

            case 10:
                message = "������� ������� �� " + FormatNumsHelper.FormatNum(task) + " �����";
                break;

            case 11:
                message = "�������� " + FormatNumsHelper.FormatNum(task) + " ����� �� �������";
                break;

            case 12:
                message = "�������� " + FormatNumsHelper.FormatNum(task) + " ����� � �������";
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
            0 => "��������� " + FormatNumsHelper.FormatNum(task) + " � �� " + planeName,
            1 => "�������� " + FormatNumsHelper.FormatNum(task) + " ����� �� " + planeName,
            2 => "�������� " + FormatNumsHelper.FormatNum(task) + " ����� �� " + planeName,
            3 => "�������� � ���� " + FormatNumsHelper.FormatTime(task) + " �� " + planeName,
            4 => "�������� " + task + " �������" + " �� " + planeName,
            _ => "error",
        };
        return message;
    }
}