mergeInto(LibraryManager.library, {
  GetCsrfTokenInternal: function () {
    var csrfToken = GetCsrfToken();
    var bufferSize = lengthBytesUTF8(csrfToken) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(csrfToken, buffer, bufferSize);
    return buffer;
  },
});