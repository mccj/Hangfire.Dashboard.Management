var zhcnlocale = moment.localeData('zh-cn');
moment.locale('zh', {
    months: zhcnlocale._months,
    monthsShort: zhcnlocale._monthsShort,
    weekdays: zhcnlocale._weekdays,
    weekdaysShort: zhcnlocale._weekdaysShort,
    weekdaysMin: zhcnlocale._weekdaysMin,
    longDateFormat: zhcnlocale._longDateFormat,
    meridiemParse: zhcnlocale._meridiemParse,
    meridiemHour: zhcnlocale.meridiemHour,
    meridiem: zhcnlocale.meridiem,
    calendar: zhcnlocale._calendar,
    dayOfMonthOrdinalParse: zhcnlocale._dayOfMonthOrdinalParse,
    ordinal: zhcnlocale.ordinal,
    relativeTime: zhcnlocale._relativeTime,
    week: zhcnlocale._week
});