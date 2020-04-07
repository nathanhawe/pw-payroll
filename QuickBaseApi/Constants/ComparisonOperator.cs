using System;
using System.Collections.Generic;
using System.Text;

namespace QuickBase.Api.Constants
{
	/// <summary>
	/// Comparison operators supported by Quick Base as documented in API_DoQuery <see href="https://help.quickbase.com/api-guide/do_query.html#queryOperators"/>here</see>.
	/// </summary>
	public enum ComparisonOperator
	{
		Unknown = 0,
		/// <summary>
		/// Contains either a specific value or the value in another field of the same type. (Do not use this operator with List - User and Multi-select Text fields; instead, use HAS.)
		/// </summary>
		CT,

		/// <summary>
		/// Does not contain either a specific value, or the value in another field of the same type. (Do not use this operator with List - User and Multi-select Text fields; instead, use XHAS.)
		/// </summary>
		XCT,

		/// <summary>
		/// Used with List - User and Multi-select Text fields only. Specifies that the field contains a specific set of values.
		/// For each user you are trying to find in a List - User field, you must enter the user's ID, user name, or email address. 
		/// You can also enter placeholder names. Be sure to surround placeholder names with double quotes.
		/// The query parameter must be surrounded by single quotes. Separate values in the list using a semi-colon.
		/// For example:
		/// <query>{'6'.HAS.'-8675309; -9873297'}</query>
		/// </summary>
		HAS,

		/// <summary>
		/// Used with List - User and Multi-select Text fields only. Specifies that the field does not contain a specific set of values. 
		/// See Filtering records using fields with multiple values for more information.
		/// For each user you are trying to find in a List - User field, the query parameter must contain the user's ID, email address, or user name. 
		/// You can also specify a placeholder name. Placeholder names must be enclosed in double quotes.
		/// The entire query parameter must be surrounded by single quotes. Separate values in the list using semi-colons.
		/// Note that a matching record must contain all users you specify.This query:
		/// <query>{'6'.XHAS. '-8675309; -9873297'}</query>
		/// ...specifies that you want to see records that do not contain BOTH of these users.Therefore, the query will return records that contain either one or neither, 
		/// but not both, of these users.
		/// </summary>
		XHAS,

		/// <summary>
		/// Is equal to either a specific value, or the value in another field of the same type.
		/// When specifying values to query from List - User and Multi-select Text fields, enclose the entire query parameter in single quotes. 
		/// Separate the values you're looking for using semi-colons. 
		/// See <see href="https://help.quickbase.com/user-assistance/multi_select_filter.html">Filtering records using fields with multiple values for more information</see>.
		/// </summary>
		EX,

		/// <summary>
		/// True Value (compares against the underlying foreign key or record ID stored in relationship fields.) Also used for queries on user fields.
		/// </summary>
		TV,

		/// <summary>
		/// Is not equal to either a specific value, or the value in another field of the same field type.
		/// When specifying values to query from List - User and Multi-select Text fields, enclose the entire query parameter in single quotes. 
		/// Separate the values you're looking for using semi-colons.
		/// </summary>
		XEX,

		/// <summary>
		/// Starts with either a specific value or the value in another field of the same type.
		/// </summary>
		SW,

		/// <summary>
		/// Does not start with either a specific value or the value in another field of the same type.
		/// </summary>
		XSW,

		/// <summary>
		/// Is before either a specific value or the value in another field of the same type.
		/// </summary>
		BF,

		/// <summary>
		/// Is on or before either a specific date or the value in another date field.
		/// </summary>
		OBF,

		/// <summary>
		/// Is after either a specific date or the value in another date field.
		/// </summary>
		AF,

		/// <summary>
		/// Is on or after either a specific date or the value in another date field
		/// </summary>
		OAF,

		/// <summary>
		/// Is during. Use this operator with date and date/time fields, to determine whether a particular date falls within particular date range relative to the current date. 
		/// Learn more about relative date ranges <see href="https://help.quickbase.com/api-guide/Understanding_relative_date_ranges.html">here</see>.
		/// </summary>
		IR,

		/// <summary>
		/// Is not during. Use this operator with date and date/time fields, to determine whether a particular date does not fall within a 
		/// particular date range relative to the current date. Learn more about relative date ranges <see href="https://help.quickbase.com/api-guide/Understanding_relative_date_ranges.html">here</see>.
		/// </summary>
		XIR,

		/// <summary>
		/// Is less than either a specific value or the value in another field of the same type.
		/// </summary>
		LT,

		/// <summary>
		/// Is less than or equal to either a specific value or the value in another field of the same type.
		/// </summary>
		LTE,

		/// <summary>
		/// Is greater than either a specific value or the value in another field of the same type.
		/// </summary>
		GT,

		/// <summary>
		/// Is greater than or equal to either a specific value or the value in another field of the same type.
		/// </summary>
		GTE


	}
}
