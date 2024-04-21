using ScienceAtrium.Domain.OrderAggregate;

namespace ScienceAtrium.Presentation.OrderAggregate.Helpers;

public static class OrderHelper
{
	public static string GetTranslatedStatus(OrderStatus orderStatus)
	{
		if (orderStatus == OrderStatus.PaymentWait)
			return "Ожидание оплаты";

		if (orderStatus == OrderStatus.Pending)
			return "В обработке";

		if (orderStatus == OrderStatus.Fulfilled)
			return "Завершен";

		if (orderStatus == OrderStatus.Cancelled)
			return "Отменен";

		if (orderStatus == OrderStatus.Delayed)
			return "Отложен";

		if (orderStatus == OrderStatus.Expired)
			return "Нарушены сроки";

		return "???";
	}
}
