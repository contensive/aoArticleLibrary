function getContent()

	dim stream
	dim cs
	dim recordID
	dim categoryID
	dim keyWord
	dim rowClass
	dim rowCount
	Dim pageReference
	dim layoutID
	dim cs2
	dim hidePrint
	dim initialrecords
	dim initialCategory
	dim copy
	dim uploadFile
	dim target
	dim link
	dim criteria

	set cs = cp.csNew
	set cs2 = cp.csNew
	articleLibraryId = cp.doc.getInteger( "article library" )
	if ( articleLibraryId>0 ) then
		criteria = "(articleLibraryId=" & articleLibraryId & ")"
	else
		criteria = "(1=1)"
	end if
	recordID = cp.utils.encodeInteger(cp.doc.var("rec"))
	categoryID = cp.utils.encodeInteger(cp.doc.var("cat"))
	keyWord = cp.doc.var("key")
	pageReference = CP.Request.Page & "?" & CP.Request.QueryString
	hidePrint = cp.utils.EncodeBoolean(cp.doc.var("hideprint"))
	initialrecords = cp.utils.encodeInteger(cp.doc.var("Initial Articles to Display"))
	initialCategory = cp.utils.encodeInteger(cp.doc.var("Initial Article Library Category"))

	if recordID <> 0 then

		if cs.Open("Article Library Data", criteria & "and(ID=" & recordID & ")") then
			stream = stream & cs.GetEditLink()
			stream = stream & cp.html.h1(cs.GetText("Name"),,"articleHead")
			stream = stream & cp.html.div(cs.GetText("Copy"),,"articleCopy")
			'
			uploadFile = cs.GetText("uploadFileName")
			link = cs.getText("link")
			'
			if uploadFile <> "" then
				stream = stream & "<div class=""articleFileCon""><p><a target=""_blank"" href=""" & cp.site.FilePath & uploadFile & """>Click here to view the resource</a></p></div>"
			end if
			if link <> "" then
				if instr( link, "://" )=0 then
					link = "http://" & link
				end if
				stream = stream & "<div class=""articleLinkCon""><p><a target=""_blank"" href=""" & link & """>Click here to view the resource</a></p></div>"
			end if
		end if
		call cs.Close()

		link = cp.Utils.ModifyLinkQueryString(pageReference, "rec", 0, True)
		link = cp.Utils.ModifyLinkQueryString(link, "button", "Search", True)
		stream = stream & cp.html.div("<a href=""" & link & """>Return to Search Results</a>",,"returnContainer")
	end if
	'
	if cp.doc.var("button") = "Search" then
		if keyWord <> "" then
			criteria = criteria & "and((name like " & cp.db.encodeSQLText("%" & keyword & "%") & ")"
			criteria = criteria & " OR (copy like " & cp.db.encodeSQLText("%" & keyword & "%") & "))"
		end if
		if categoryID <> 0 then
			criteria = criteria & "and(articleLibraryCategoryID=" & categoryID &")"
		end if
		'
		if cs.Open("Article Library Data", criteria,"name") then
			stream = stream & "<table class=""summaryTable"">"
			stream = stream & "<tr>"
			stream = stream & "<td class=""header"">Name</td>"
			stream = stream & "<td class=""header"">Category</td>"
			stream = stream & "</tr>"
			do while cs.OK()
	            		If (rowCount Mod 2) = 0 Then
	                		rowClass = "summaryOdd"
	            		Else
	                		rowClass = "summaryEven"
	            		End If
				'
				uploadFile = cs.GetText("uploadFileName")
				copy = cs.GetText("copy")
				'
				if uploadFile<>"" and copy="" then
					link = cp.Site.FilePath & uploadFile
					target = "_blank"
				else
					link = cp.Utils.ModifyLinkQueryString(pageReference, "rec", cs.GetInteger("ID"), True)
					link = cp.Utils.ModifyLinkQueryString(link, "key", keyword, True)
					link = cp.Utils.ModifyLinkQueryString(link, "cat", categoryID, True)
					target = "_self"
				end if
				'
				stream = stream & "<tr>"
				stream = stream & "<td class=""" & rowClass & "Left"">" & cs.GetEditLink() & "<a href=""" & link & """>" & cs.GetText("Name") & "</a></td>"
				stream = stream & "<td class=""" & rowClass & "Right"">" & cs.GetText("articleLibraryCategoryID") & "</td>"
				stream = stream & "</tr>"
				cs.goNext()
				rowCount = rowCount + 1
			loop
			stream = stream & "</table>"
		else
			stream = stream & cp.html.p("Your search returned no results",,"ccError")
		end if
		cs.Close()
		'
	elseif recordID = 0 then

		call cp.site.SetProperty("News Page Location", CP.Request.Page & "?" & CP.Request.QueryString)

		if initialrecords <> 0 then
			'
			'	if selected only show selected category initially
			'
			if initialCategory <> 0 then
				criteria = criteria & "and(articleLibraryCategoryID=" & initialCategory & ")"
			end if
			'
			call cs.Open("Article Library Data", criteria,"DateAdded desc")
			stream = stream & "<table class=""summaryTable"">"
			stream = stream & "<tr>"
			stream = stream & "<td class=""header"">Name</td>"
			stream = stream & "<td class=""header"">Category</td>"
			stream = stream & "</tr>"
			do while (cs.OK()) and (rowCount<=initialrecords)
		         	If (rowCount Mod 2) = 0 Then
		               		rowClass = "summaryOdd"
		           	Else
		               		rowClass = "summaryEven"
		           	End If
				'
				uploadFile = cs.GetText("uploadFileName")
				copy = cs.GetText("copy")
				'
				if uploadFile<>"" and copy="" then
					link = cp.Site.FilePath & uploadFile
					target = "_blank"
				else
					link = cp.Utils.ModifyLinkQueryString(pageReference, "rec", cs.GetInteger("ID"), True)
					link = cp.Utils.ModifyLinkQueryString(link, "key", keyword, True)
					link = cp.Utils.ModifyLinkQueryString(link, "cat", categoryID, True)
					target = "_self"
				end if
				'
				stream = stream & "<tr>"
				stream = stream & "<td class=""" & rowClass & "Left"">" & cs.GetEditLink() & "<a target=""" & target & """ href=""" & link & """>" & cs.GetText("Name") & "</a></td>"
				stream = stream & "<td class=""" & rowClass & "Right"">" & cs.GetText("articleLibraryCategoryID") & "</td>"
				stream = stream & "</tr>"
				call cs.goNext()
				rowCount = rowCount + 1
			loop
			stream = stream & "</table>"
			call cs.Close()
			'
			stream = stream & "<img src=""/ccLib/images/spacer.gif"" width=""10"" height=""20"">"
		end if

		stream = stream & cp.html.div("Keyword:",,"fieldCaption")
		stream = stream & cp.html.div(cp.Html.inputText("key","",,"35"),,"fieldData")

		stream = stream & cp.html.div("Category:",,"fieldCaption")
		stream = stream & cp.html.div(cp.Html.SelectContent("cat","","Article Library Categories",,"Select One"),,"fieldData")

		stream = stream & cp.html.div(cp.html.button("button", "Search"),,"buttonContainer")

		stream = cp.html.form(stream)
	end if

	if ((recordID <> 0) OR (cp.doc.var("button") = "Search")) and (not hidePrint) then
		link = cp.utils.ModifyLinkQueryString(pageReference, "rec", 0)
		link = cp.Utils.ModifyLinkQueryString(link, "key", "")
		link = cp.Utils.ModifyLinkQueryString(link, "cat", 0)
		link = cp.Utils.ModifyLinkQueryString(link, "button", "")
		stream = stream & cp.html.div("<a href=""" & link & """>Create another search</a>",,"returnContainer")
	end if
	'stream = stream & "<br>criteria=[" & criteria & "]"
	getContent = cp.html.div(stream, ,"libraryContainer")

end function
