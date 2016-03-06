namespace Zuris.SPDAL
{
    public class DataManagerSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether [enable read command logging].
        /// </summary>
        /// <value>
        /// <c>true</c> if [enable read command logging]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableReadCommandLogging { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [enable write command logging].
        /// </summary>
        /// <value>
        /// <c>true</c> if [enable write command logging]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableWriteCommandLogging { get; set; }

        /// <summary>
        /// Gets a value indicating whether [command logging enabled].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [command logging enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool CommandLoggingEnabled
        {
            get { return EnableReadCommandLogging || EnableWriteCommandLogging; }
        }
    }
}