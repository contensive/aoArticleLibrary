function getContent()

	dim stream
	dim cs
	dim rowCount
	Dim pageReference
	dim recordsToDisplay
	dim articleLibraryId
	dim criteria

	recordsToDisplay = cp.utils.encodeInteger(cp.doc.var("Records to Display"))
	articleLibraryId = cp.doc.getInteger( "article Library" )
	if ( articleLibraryId>0 ) then
		criteria = "(articleLibraryId=" & articleLibraryId & ")"
	else
		criteria = "(1=1)"
	end if

	if recordsToDisplay = 0 then
		recordsToDisplay = 5
	end if

	rowCount = 1
	pageReference = cp.site.getProperty("News Page Location")

	set cs = cp.csNew
	call cs.Open("Article Library Data",criteria,"DateAdded desc")
	do while (cs.OK()) and (rowCount<=recordsToDisplay)
		link = cp.Utils.ModifyLinkQueryString(pageReference, "rec", cs.GetInteger("ID"), True)
		link = cp.Utils.ModifyLinkQueryString(link, "key", keyword, True)
		link = cp.Utils.ModifyLinkQueryString(link, "cat", categoryID, True)
		stream = stream & cp.html.li("<a href=""" & link & """>" & cs.GetText("Name") & "</a>")
		call cs.goNext()
		rowCount = rowCount + 1
	loop
	call cs.Close()

	getContent = cp.html.ul(stream,,"libraryList")

end function